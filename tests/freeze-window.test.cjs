const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');
const scriptPath = path.join(__dirname, '../Hub.Legacy/Gcpe.Hub.Legacy.Website/Calendar/Scripts/activityhelper.js');
function fixture(utcIso) {
  const clock = Date.parse(utcIso);
  const requests = [], alerts = [], actions = [];
  function $(selector) {
    const item = {length: 1};
    for (const operation of ['show','hide','remove','prop','css'])
      item[operation] = (...args) => {actions.push({selector,operation,args});return item;};
    return item;
  }
  $.ajax = options => { if (options.url.includes('ChangeFreezeWindow')) requests.push(options); };
  class BrowserDate extends Date {
    constructor(...args) { super(...(args.length ? args : [clock])); }
    static now() { return clock; }
    toLocaleTimeString() { throw new Error('Browser time zone data must not decide the pre-check'); }
  }
  const context = { $, Date: BrowserDate, alert: message => alerts.push(message), navigator: {}, window: {} };
  vm.createContext(context);
  vm.runInContext(fs.readFileSync(scriptPath,'utf8'),context);
  return {context, requests, alerts, actions};
}
function runInitialLoad(f) {
  const page=fs.readFileSync(path.join(__dirname,'../Hub.Legacy/Gcpe.Hub.Legacy.Website/Calendar/Activity.aspx'),'utf8');
  const start=page.indexOf('            highlightChangedFields();')+'            highlightChangedFields();'.length;
  vm.runInContext(page.slice(start,page.indexOf('            // Add tool tips')),f.context);
}
// Summary: both BC offsets fit the coarse UTC 23/0-hour pre-check; requests disable caching.
test('UTC pre-check asks server around 16:05-17:00 BC at UTC-7 and UTC-8', () => {
  // 16:30 PCT, 16:30 historical PST, and the minutes just inside each hour boundary.
  for (const utc of ['2026-12-01T23:30:00Z','2025-12-01T00:30:00Z','2026-12-01T23:00:00Z','2026-12-02T00:59:59Z']) {
    const f=fixture(utc);
    f.context.SetChanged('activity');
    assert.equal(f.requests.length,1,utc);
    assert.equal(f.requests[0].cache,false,utc);
  }
});
// Summary: no request outside the coarse window, including the former morning false positive.
test('UTC pre-check skips the server outside the coarse window', () => {
  // 15:59 PCT, 18:00 PCT, and 04:30 PCT (the old string-comparison false positive).
  for (const utc of ['2026-12-01T22:59:59Z','2026-12-02T01:00:00Z','2026-12-01T11:30:00Z']) {
    const f=fixture(utc);
    f.context.SetChanged('activity');
    assert.equal(f.requests.length,0,utc);
  }
});
// Summary: execute the actual Activity.aspx initial-load fragment inside and outside the window.
test('first page load uses the same pre-check before asking the server', () => {
  const inWindow=fixture('2026-12-01T23:30:00Z');
  runInitialLoad(inWindow);
  assert.equal(inWindow.requests.length,1);
  const outside=fixture('2026-12-01T18:00:00Z');
  runInitialLoad(outside);
  assert.equal(outside.requests.length,0);
});
// Summary: a True response alerts and removes edit actions; False leaves controls untouched.
test('server true disables editing; false leaves controls alone', () => {
  const f=fixture('2026-12-01T23:30:00Z');f.context.checkDailyChangeFreeze();f.requests[0].success('True');
  assert.equal(f.alerts.length,1);
  assert.ok(f.actions.some(a=>a.selector==='#ActionsFieldset' && a.operation==='remove'));
  const open=fixture('2026-12-01T23:30:00Z');open.context.checkDailyChangeFreeze();open.requests[0].success('False');
  assert.equal(open.alerts.length,0);
  assert.equal(open.actions.length,0);
});
