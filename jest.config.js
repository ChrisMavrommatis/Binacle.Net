// Runs every workspace's tests from the repo root, so coverage paths carry the workspace folder
// (vipaq/packages/binacle-vipaq/src/x.ts, not src/x.ts). Two workspaces with a file at the same relative
// path would otherwise collide and report 0% — see the monorepo note in Sonar's coverage docs.
//
// collectCoverageFrom has to live here, not in the per-package configs: in multi-project mode jest ignores
// the projects' copies, which silently pulls test helpers and barrels back into the numbers.
/** @type {import("jest").Config} **/
module.exports = {
  projects: [
    "<rootDir>/packages/binacle-compact-notation",
    "<rootDir>/vipaq/packages/binacle-vipaq",
  ],
  // These globs resolve against each project's own rootDir, not this file's — repo-root-relative paths
  // here match nothing and produce an empty report.
  collectCoverageFrom: [
    "src/**/*.ts",
    "!src/**/index.ts",
  ],
};
