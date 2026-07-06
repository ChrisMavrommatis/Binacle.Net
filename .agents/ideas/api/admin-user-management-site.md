# Idea: Admin site for user management

**Status:** Unvetted idea.

## What

An admin site to manage users. It can register users and manage the subscriptions that already exist in the
system. A place to see and control who has access.

## Why

Right now user/subscription state lives in the ServiceModule but there's no front door to manage it. An admin
site gives us one — register a user, look up a subscription, change a tier, without touching the database by
hand.

## Notes

- This should build on the ServiceModule, which already does JWT auth and account/subscription management.
  Check what's there before adding new storage.
- Likely ties into the Shopify plugin, where installed stores become registered users
  (see [shopify-plugin.md](../shopify-plugin.md)).

## Open questions

- Who is this admin site for — us (maintainers) or customers managing their own team.
- Reuse the UIModule (Blazor) or a separate site.
- What subscription actions are in scope — create, upgrade/downgrade, cancel.

## Related

- `.agents/docs/api/modules/service.md`, `.agents/docs/api/modules/ui.md`
