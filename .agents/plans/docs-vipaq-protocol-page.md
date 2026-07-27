# Docs site - the shared ViPaq protocol page describes the old format

**Status:** Not started. Must land before v3.0.0 is publicised. Today the page tells a v3.0.0 user their tokens
are gzip, which is wrong.

**Decide this before the v3.0.x pages session starts.** The choice below is not a detail of writing the page - option
1 puts a copy of it in every version folder at once, which enlarges that session's scope. Making the call inside
that session means discovering the extra work halfway through it.

`docs/` is off limits to a coding session. This file is the brief for the docs session.

## The problem

`docs/collections/_common_pages/vipaq-protocol.md` is a `_common_pages` page shared by **every** version folder.
It is not versioned - it renders once at `/vipaq-protocol/`, and it has not changed since v2.1.1, so it describes
the old format for every reader.

The specific error: it lists **"Gzip Compression"**. The rebuilt codec is **raw DEFLATE (RFC 1951)**, with no
gzip or zlib wrapper. The rest of the page still holds for both formats.

Because there is one copy, it cannot simply be edited per version.

## The call

1. **Move it into the version folders** (`_versions/<version>/vipaq-protocol.md`) so each version describes its
   own format. Correct, but it stops being a common page and every `{% link %}` reference to it becomes a
   `vlink` - and `vlink` fails the build on a missing target, so every version folder needs a copy at once.
2. **Keep it shared and make it version-aware** - describe both formats on one page, saying which images produce
   which. Cheaper, but the page must stay honest about two incompatible formats forever.

## Also say this on the page, whichever option wins

Tokens do not cross the release. Images at v2.1.1 and earlier produce the old format; v3.0.0 onward produces and
reads only the new one, and there is no fallback reader. A user running an old and a new image side by side will
find their tokens do not interoperate. An old token fails loudly with a format error rather than being misread -
that is verified and pinned by regression vectors.

## Done when

The page is correct for whichever version a reader is on, and it says the two formats do not mix.
