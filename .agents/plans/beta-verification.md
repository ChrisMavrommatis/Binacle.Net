# Beta verification - what to check while the v3.0.0 beta is deployed

**Status:** Not started. This is the whole point of shipping a beta image first. The beta is the first time this
code runs anywhere outside a test host, and three of the release's changes only fail in a real deployment:
forwarded headers, the health check allow-list, and rate limiting on the resolved caller.

A beta nobody observes is just an extra docker tag. Work this list while it is up, and record the answers here
before the file is deleted.

## Before deploying

- Note the currently running image tag, so there is a known-good version to roll back to.
- Rolling back is repointing the deployment at the previous tag - there is no data migration in this release, so
  a rollback is safe.
- Keep the deployment host and its layout out of this file and out of the repo.

## Check while it is running

- [ ] **Read the log before reaching for `/_debug`.** A forwarding header that arrives and does not take effect
      now warns once at startup, naming the header and the address being read as the caller. No warning after
      real traffic has hit the deployment means the header is being applied - which answers the next box without
      turning on an endpoint that echoes `Authorization` headers. A warning names which of the two states it is:
      feature off, or the proxy missing from the trust list.
- [ ] **The real caller is resolved.** Enable `DEBUG_ENDPOINT=True` and call `/_debug` from outside. The
      `[connection] RemoteAddr` is the proxy; the forwarding headers under `[headers]` are what the proxy sent.
      With forwarded headers enabled and the proxy trusted, `X-Original-For` appears - its absence means the
      trust list does not match, and the app is treating the proxy as the caller.
- [ ] **Turn `/_debug` back off** once the answer is recorded. It echoes the caller's own request, including
      their `Authorization` header.
- [ ] **The health check allow-list matches.** Call the health endpoints from an address that is inside the list
      and one that is not. This is the change most likely to lock someone out: CIDR now means a prefix length,
      so an entry that used to cover nearly the whole IPv4 range now covers what it says.
- [ ] **Login rate limiting partitions on the real caller.** Repeated auth attempts from one client must hit the
      limit, and varying `X-Forwarded-For` must not reset it.
- [ ] **Startup validation is quiet.** No config warnings in the startup log for the flattened packing-logs
      shape or the forwarded-headers trust settings. Both `RestrictedIPs` and `TrustedProxies` are now read
      exactly as written, so an entry with a leading zero (`010.10.10.10`, which used to mean `8.10.10.10`) or a
      written-out IPv6 address stops the app rather than being reinterpreted. If the deployment carries either,
      this is where it shows up. An entry whose host bits were set (`192.168.1.1/24`) starts fine and logs what
      it resolved to - check that line says the block you meant.
- [ ] **Packing logs land in the new path.** `data/pack-logs/` directly, not `data/pack-logs/packing/`, and
      entries carry a `Timestamp`.
- [ ] **ViPaq tokens round-trip.** Produce a token from the beta, decode it in the beta's UI Protocol Decoder.
      An old token from the previous image must be rejected loudly, not misread.
- [ ] **V4 is served and marked experimental.** Fetch `/openapi/v4.json` and confirm the warning banner is in
      the document description.

## Done when

Every box is ticked and anything surprising has been turned into a defect or an accepted note in the release
file. Then the final tag can go out.
