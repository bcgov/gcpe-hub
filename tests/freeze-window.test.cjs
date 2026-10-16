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
// Summary: request only during 16:00-17:00 BC at UTC-7; requests disable caching.
test('browser asks server during 16:00-17:00 BC at UTC-7, now and in future winters', () => {
  // BC 16:00, 16:30 and 16:59:59; current, post-switch and future dates all use UTC-7.
  for (const utc of ['2026-10-15T23:00:00Z','2026-12-01T23:30:00Z','2026-12-01T23:59:59Z','2027-01-15T23:30:00Z','2027-07-01T23:30:00Z']) {
    const f=fixture(utc);
    f.context.SetChanged('activity');
    assert.equal(f.requests.length,1,utc);
    assert.equal(f.requests[0].cache,false,utc);
  }
});
// Summary: no request outside the coarse window, including the former morning false positive.
test('UTC pre-check skips the server outside the coarse window', () => {
  // BC 15:59:59, 17:00, 17:30, 17:59:59, 18:00 and the old morning false positive.
  for (const utc of ['2026-12-01T22:59:59Z','2026-12-02T00:00:00Z','2026-12-02T00:30:00Z','2026-12-02T00:59:59Z','2026-12-02T01:00:00Z','2026-12-01T11:30:00Z']) {
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
  for (const utc of ['2026-12-01T22:59:59Z','2026-12-02T00:00:00Z','2026-12-02T00:30:00Z']) {
    const outside=fixture(utc);
    runInitialLoad(outside);
    assert.equal(outside.requests.length,0,utc);
  }
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
