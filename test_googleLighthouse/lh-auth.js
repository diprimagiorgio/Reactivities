/**
 * @license Copyright 2019 The Lighthouse Authors. All Rights Reserved.
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
 */
 'use strict';

 /**
  * @fileoverview Example script for running Lighthouse on an authenticated page.
  * See docs/recipes/auth/README.md for more.
  */


 
 // This port will be used by Lighthouse later. The specific port is arbitrary.
 const PORT = 3001;
 let counter = 1;
 /**
  * @param {import('puppeteer').Browser} browser
  * @param {string} origin
  */
 async function login(page) {
   
   await page.waitForSelector('button[name="login-button"]', {visible: true});
   const login = await page.$('button[name="login-button"]');
   login.click();
   await page.waitForSelector('input[name="email"]', {visible: true});
   await page.waitForSelector('input[name="password"]', {visible: true});
 
   // Fill in and submit login form.
   const emailInput = await page.$('input[name="email"]');
   await emailInput.type('bob@test.com');
   const passwordInput = await page.$('input[type="password"]');
   await passwordInput.type('Pa$$w0rd');
   await Promise.all([
     page.$eval('button[type="submit"]', form => form.click()),
     page.waitForNavigation(),
   ]);
  }
   
 
  /**
  * @param {puppeteer.Browser} browser
  */
 async function logout(page, context) {
 
   await page.goto(`${context.url}/logout`);
   
 }

 /**
 * @param {puppeteer.Browser} browser
 * @param {{url: string, options: LHCI.CollectCommand.Options}} context
 */
   async function setup(browser, context) {

    // launch browser for LHCI
    const page = await browser.newPage();
    await page.setCacheEnabled(true)
    if(counter===1){
      await logout(page, context)
      page.goto(context.url)
      await login(page)
    }
    else if(counter === 3){
      await logout(page)
    }
    else{
        await page.goto(context.url);
    }
    // close session for next run
    await page.close();
    counter++
 }
 
 
 module.exports = setup