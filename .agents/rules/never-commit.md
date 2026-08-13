---
description: The human commits, stages and pushes. An agent never does.
load: always
when: before any git command, and before saying a task is finished
---

# Never commit, stage or push

Leave every change in the working tree. This holds even when the task is done and the build is green.

**Why:** the maintainer reviews by staging as they read. An agent that stages or commits takes that
review away, and an unwanted commit costs more to undo than to never make.
