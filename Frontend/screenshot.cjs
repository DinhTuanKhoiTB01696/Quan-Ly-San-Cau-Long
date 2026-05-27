const puppeteer = require('puppeteer');
const path = require('path');

(async () => {
  const browser = await puppeteer.launch({
    headless: "new"
  });
  const page = await browser.newPage();
  
  // Set viewport for a nice screenshot
  await page.setViewport({ width: 1280, height: 800 });

  console.log("Navigating to login...");
  await page.goto('http://localhost:5173/login', { waitUntil: 'networkidle2' });

  // Login as admin
  await page.type('input[type="text"]', 'admin');
  await page.type('input[type="password"]', 'Admin@123');
  await page.click('button[type="submit"]');

  console.log("Waiting for navigation to home...");
  await page.waitForNavigation({ waitUntil: 'networkidle2' });

  // Navigate to admin
  console.log("Navigating to admin...");
  await page.goto('http://localhost:5173/admin', { waitUntil: 'networkidle2' });
  
  // Wait a bit for charts to render
  await new Promise(r => setTimeout(r, 2000));

  // Take screenshot of Dashboard Tab
  const dashboardPath = path.join(__dirname, 'admin_dashboard.png');
  await page.screenshot({ path: dashboardPath, fullPage: true });
  console.log(`Saved screenshot to ${dashboardPath}`);

  // Click on Users Tab
  await page.evaluate(() => {
    const tabs = document.querySelectorAll('.admin-tabs button');
    for (let t of tabs) {
      if (t.innerText === 'Người Dùng') t.click();
    }
  });
  await new Promise(r => setTimeout(r, 1000));
  
  const usersPath = path.join(__dirname, 'admin_users.png');
  await page.screenshot({ path: usersPath, fullPage: true });
  console.log(`Saved screenshot to ${usersPath}`);

  // Click on Matches Tab
  await page.evaluate(() => {
    const tabs = document.querySelectorAll('.admin-tabs button');
    for (let t of tabs) {
      if (t.innerText === 'Quản lý Kèo') t.click();
    }
  });
  await new Promise(r => setTimeout(r, 1000));

  const matchesPath = path.join(__dirname, 'admin_matches.png');
  await page.screenshot({ path: matchesPath, fullPage: true });
  console.log(`Saved screenshot to ${matchesPath}`);

  await browser.close();
})();
