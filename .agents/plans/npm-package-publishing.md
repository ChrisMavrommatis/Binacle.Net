# Decide whether the TypeScript packages get published

**Status:** Not started. After v3.0.0, but the release is what makes it a real question.

## Why

Two workspace packages are **not** marked `private` and have no publish workflow:

- `vipaq/packages/binacle-vipaq` - version `1.0.0`
- `packages/binacle-compact-notation` - version `1.0.0`

Every other workspace package (`cookies`, `binacle-net-ui`, `theme-switcher`, and the root) is `private: true`.
So these two are either an oversight or an unfinished intention, and nothing in the repo says which.

It matters now because v3.0.0 rebuilds the ViPaq format and rejects every token an earlier version produced.
`binacle-vipaq` is the reference decoder for the new format. A JavaScript consumer who wants to read a v3 token
has no supported way to get it - the answer today is "vendor the source out of our repo".

## The call

1. **Publish `binacle-vipaq` to npm.** It becomes a real dependency with a version contract, which means the
   cross-language test vectors become a compatibility promise, not just an internal check. Needs a publish
   workflow, an npm org, a README, and a version policy tied to the wire format rather than to the API version.
2. **Mark both `private: true`.** They are internal, the site and the UI package are the only consumers, and a
   JS user is expected to call the API rather than decode tokens locally.

Pick one. The current state is neither, and `npm publish` from the wrong directory is all it takes to make the
decision by accident.

## If publishing wins

- Decide the version relationship: the package version tracks the ViPaq wire format, not the Binacle.Net
  release. They move at different speeds and this release is proof.
- The wire format is documented in `vipaq/PROTOCOL.md` - a published package needs that reachable from its
  README.
- `binacle-compact-notation` is a separate decision. It may well stay internal even if `binacle-vipaq` ships.

## Done when

Both packages either publish deliberately or say `private: true`.
