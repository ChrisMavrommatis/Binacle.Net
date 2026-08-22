const { createDefaultPreset } = require("ts-jest");

const tsJestTransformCfg = createDefaultPreset().transform;

/** @type {import("jest").Config} **/
module.exports = {
  // The npm package name, so `--selectProjects <name>` can run this package on its own through the
  // root config, and so coverage and test reports are filed under a name that means something.
  displayName: "binacle-compact-notation",
  testEnvironment: "node",
  transform: {
    ...tsJestTransformCfg,
  },
  testMatch: ["**/tests/**/*.test.ts"],
};
