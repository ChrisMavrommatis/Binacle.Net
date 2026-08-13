---
name: test-leaf-naming
description: How a test leaf is named - <slice>[-<component>][-<language>]-<kind>, kind spelled out, no two leaves a letter apart
type: convention
when: adding or renaming a test leaf
paths:
  - "tooling/tests.just"
  - "**/test/**"
---

A test leaf is one project and one run, so it is also one CI step. Its recipe name follows:

```
<slice>[-<component>][-<language>]-<kind>
```

- **The kind is spelled out** - `unit` and `integration`, never `ut` / `it`.
- **The language segment appears only where a slice has both a C# project and a TS package.** `lib` has one
  language, so the leaf is `lib-unit`, not `lib-cs-unit`. `shared` and `vipaq` have both, so they carry
  `-cs` / `-ts`.
- **No two leaves may differ by one letter in the middle of a word**, and a prefix someone types has to
  narrow to something.

There is **one recipe per leaf**, listed by hand rather than generated. Shell completion only completes
recipe names, so a leaf passed as an argument could not complete without a hand-written list that drifts.

**Why:** these names are typed at a prompt and pasted from a red CI step, so they are read under pressure and
half-typed with tab. Two leaves a letter apart is how someone runs the wrong suite and believes a green
result.

**How to apply:** when adding a suite, name the leaf before writing it and check it against the existing list
in `tooling/tests.just`. Adding it to that module's `all` recipe is a separate judgement - `all` is the
infra-free set, so a leaf joins it only once someone confirms it passes with nothing brought up.
