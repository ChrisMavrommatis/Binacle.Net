const { createDefaultPreset } = require("ts-jest");

const tsJestTransformCfg = createDefaultPreset().transform;

/** @type {import("jest").Config} **/
module.exports = {
  // The npm package name, so `--selectProjects <name>` can run this package on its own through the
  // root config, and so coverage and test reports are filed under a name that means something.
  displayName: "theme-switcher",
  testEnvironment: "jsdom",
  // https, not jsdom's default http: the cookies package defaults to secure, and jsdom hides a secure
  // cookie from a document on an insecure origin, so the theme would never read back.
  testEnvironmentOptions: {
    url: "https://localhost/",
  },
  transform: {
    ...tsJestTransformCfg,
  },
  testMatch: ["**/tests/**/*.test.ts"],
};
