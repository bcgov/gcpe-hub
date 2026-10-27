const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');

const calendar = path.join(__dirname, '../Hub.Legacy/Gcpe.Hub.Legacy.Website/Calendar');
const source = fs.readFileSync(path.join(calendar, 'Scripts/activityhelper.js'), 'utf8');

function fixture() {
  const requests = [], alerts = [], changes = [], beacons = [], intervals = new Map();
  const fields = new Map();
  let nextTimer = 0, reply = '';
  function $(selector) {
    const item = {};
    for (const operation of ['show', 'hide', 'prop', 'next']) {
      item[operation] = (...args) => { changes.push({selector, operation, args}); return item; };
    }
    item.val = () => fields.get(selector);
    return item;
  }
  $.url = () => ({param: () => '42'});
  $.ajax = options => {
    requests.push(options);
    if (options.async === false) {
      if (reply instanceof Error) options.error();
      else options.success(reply);
    }
  };
  const context = {
    $, alert: message => alerts.push(message),
    window: {
      setInterval(callback, delay) { intervals.set(++nextTimer, {callback, delay}); return nextTimer; },
      clearInterval(id) { intervals.delete(id); },
      open() { throw new Error('A lock conflict must preserve unsaved input'); },
    },
    navigator: {sendBeacon: (url, data) => { beacons.push({url, data}); return true; }},
    FormData,
  };
  vm.createContext(context);
  vm.runInContext(source, context);
  context.activityEditingToken = '11111111-1111-4111-8111-111111111111';
  return {context, requests, alerts, changes, beacons, intervals, fields, reply: value => { reply = value; }};
}

test('new activities do not acquire a lock or start a heartbeat', () => {
  const f = fixture();
  assert.equal(f.context.GetActivityStatus(), true);
  assert.equal(f.requests.length, 0);
  assert.equal(f.intervals.size, 0);
});

test('acquisition starts one heartbeat and a save recheck renews the same page token', () => {
  const f = fixture();
  assert.equal(f.context.GetActivityStatus('42'), true);
  assert.equal(f.context.GetActivityStatus('42'), true);
  assert.equal(f.intervals.size, 1);
  const timer = [...f.intervals.values()][0];
  assert.equal(timer.delay, 60000);
  timer.callback();
  const renew = f.requests.at(-1);
  assert.match(renew.url, /RenewEditingActivity/);
  assert.equal(renew.data.activityId, '42');
  assert.equal(renew.data.editingToken, f.context.activityEditingToken);
});

test('a denied acquisition preserves input and prevents saving', () => {
  const f = fixture();
  f.reply('Another editor has this activity.');
  assert.equal(f.context.GetActivityStatus('42'), false);
  assert.equal(f.intervals.size, 0);
  assert.equal(f.alerts.length, 1);
  assert.ok(f.changes.some(c => c.selector.includes('#SaveButton') && c.operation === 'prop' && c.args[1] === true));
});

test('a lost lease stops heartbeats, keeps input, and alerts only once', () => {
  const f = fixture();
  f.context.GetActivityStatus('42');
  [...f.intervals.values()][0].callback();
  const renew = f.requests.at(-1);
  renew.success('Your editing session has ended.');
  renew.success('Your editing session has ended.');
  assert.equal(f.intervals.size, 0);
  assert.equal(f.alerts.length, 1);
});

test('a transient network failure does not discard input or stop retries', () => {
  const f = fixture();
  f.context.GetActivityStatus('42');
  [...f.intervals.values()][0].callback();
  assert.equal(f.intervals.size, 1);
  assert.equal(f.alerts.length, 0);
  f.reply(new Error('offline'));
  assert.equal(f.context.GetActivityStatus('42'), false);
});

test('release identifies the owning page and stops its heartbeat', () => {
  const f = fixture();
  f.context.GetActivityStatus('42');
  f.context.DoCancel('42');
  assert.equal(f.intervals.size, 0);
  assert.equal(f.beacons.length, 1);
  assert.equal(f.beacons[0].data.get('activityId'), '42');
  assert.equal(f.beacons[0].data.get('editingToken'), f.context.activityEditingToken);
});

test('page navigation releases a lock, but postback navigation leaves it for the response', () => {
  const f = fixture();
  const events = new Map();
  f.context.window.addEventListener = (name, callback) => events.set(name, callback);
  f.context.ConfirmUnsavedChanges = () => {};
  f.context.ActivityEditingToken = f.context.activityEditingToken;
  const page = fs.readFileSync(path.join(calendar, 'Activity.aspx'), 'utf8');
  const start = page.indexOf('        window.onbeforeunload = ConfirmUnsavedChanges;');
  const end = page.indexOf('        function CalculateMaxEndDate', start);
  vm.runInContext(page.slice(start, end).replace('<%= ActivityEditingToken %>', f.context.activityEditingToken), f.context);
  events.get('pagehide')({});
  assert.equal(f.beacons.length, 1);
  f.fields.set('#IsPostBackHiddenField', 'true');
  events.get('pagehide')({});
  assert.equal(f.beacons.length, 1);
  f.context.CancelEditing();
  assert.equal(f.beacons.length, 2, 'explicit cancel still releases during a postback');
  f.context.isChanged = true;
  events.get('pageshow')({persisted: true});
  assert.match(f.requests.at(-1).url, /GetActivityStatus/);
});

test('fifteen idle ticks release the lease even if the browser will not close the tab', () => {
  const f = fixture();
  const events = {};
  const originalDollar = f.context.$;
  const idleDollar = selector => {
    if (selector !== f.context.document) return originalDollar(selector);
    return {
      ready(callback) { callback.call(f.context.document); },
      mousemove(callback) { events.mousemove = callback; },
      keypress(callback) { events.keypress = callback; },
    };
  };
  idleDollar.ajax = originalDollar.ajax;
  f.context.document = {};
  f.context.$ = idleDollar;
  f.context.setInterval = f.context.window.setInterval;
  let closeAttempts = 0;
  f.context.window.open = () => ({close() { closeAttempts++; }});
  f.context.CancelEditing = () => f.context.DoCancel('42');
  f.context.GetActivityStatus('42');
  const page = fs.readFileSync(path.join(calendar, 'Activity.aspx'), 'utf8');
  const start = page.indexOf('        var idleTime = 0;');
  const end = page.indexOf('            function highlightChangedFields()', start);
  vm.runInContext(page.slice(start, end) + '\n});', f.context);
  const idleTimer = [...f.intervals.values()].at(-1);
  for (let i = 0; i < 10; i++) idleTimer.callback();
  events.mousemove();
  for (let i = 0; i < 14; i++) idleTimer.callback();
  assert.equal(f.beacons.length, 0, 'user activity resets the inactivity counter');
  idleTimer.callback();
  assert.equal(f.beacons.length, 1);
  assert.equal(closeAttempts, 1);
  assert.equal(f.intervals.size, 1, 'only the idle timer remains; heartbeat stopped');
});
