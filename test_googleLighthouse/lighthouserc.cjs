module.exports = {
  ci: {
    collect: {
      puppeteerScript: './lh-auth.js',
      puppeteerLaunchOptions: {args: [  '--allow-no-sandbox-job', '--allow-sandbox-debugging', '--no-sandbox', '--disable-gpu', '--disable-gpu-sandbox', '--display',  '--preset=desktop',  '--no-screenEmulation.mobile']}, //https://www.puppeteersharp.com/api/PuppeteerSharp.LaunchOptions.html
      numberOfRuns: 3,
      disableStorageReset: true,
      settings: {
          // Don't clear localStorage/IndexedDB/etc before loading the page.
          "disableStorageReset": true,
          // Wait up to 60s for the page to load
          "maxWaitForLoad": 60000,
          // Use applied throttling instead of simulated throttling
          "throttlingMethod": "devtools", 
          "preset": "desktop"

      },
      url: ['http://localhost:3000/' , 'http://localhost:3000/activities' ],
      emulatedFormFactor: "desktop",
      internalDisableDeviceScreenEmulation: true,
    },
    assert: {
      budgetsFile: "./budget.json"
    },
    upload: {
      target: 'temporary-public-storage',
    },
  },
};