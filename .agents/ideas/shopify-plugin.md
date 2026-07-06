# Idea: Shopify plugin

**Status:** Unvetted idea. Keep possibilities open — some of this is far out.

## What

A Shopify app/plugin that connects a store to Binacle. Our server sits in the loop and validates the Shopify
API calls (see open question on what "validate" means here).

## Why

Meet customers where they already sell. A store owner installs the plugin and gets Binacle's fit/pack answers
without wiring up the API themselves.

## Possible extension — box-usage analytics (far out, keep open)

Take in the store's boxes and items, run some analysis over what actually gets used, and show a dashboard of
most-used boxes. The store's back office can use it to know what to reorder. This is a stretch — note it now,
don't build it yet.

## Open questions

- What does "validate Shopify API calls" mean exactly — verify webhook signatures (HMAC), gate/inspect calls
  to Shopify, or act as a proxy that checks requests before they hit Shopify. Pin this down before designing.
- Where does the plugin run and how does it authenticate to Binacle — ties into ServiceModule
  (see [admin-user-management-site.md](api/admin-user-management-site.md)).
- Data ownership and privacy for the analytics extension — we'd be holding a store's box/item data.

## Related

- `.agents/docs/api/modules/service.md` (auth, subscriptions)
- [admin-user-management-site.md](api/admin-user-management-site.md)
