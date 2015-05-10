import os, re, json

from flask import Flask
from flask import request
from flask import g

import sqlite3

DATABASE = 'database.db'

allowed_tokens = ['jmLq5oA59pIu9Icg']

app = Flask(__name__)

@app.before_request
def before_request():
    g.db = sqlite3.connect(DATABASE)
    c = g.db.cursor()
    c.execute('''CREATE TABLE IF NOT EXISTS level (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT
              )''')

    c.execute('''CREATE TABLE IF NOT EXISTS level_version (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    level INTEGER,
                    hash TEXT NOT NULL,
                    FOREIGN KEY(id) REFERENCES level(id)
              )''')

    c.execute('''CREATE TABLE IF NOT EXISTS player (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    unique_identifier TEXT NOT NULL,
                    system_info TEXT NOT NULL,
                    device_model TEXT NOT NULL
              )''')

    c.execute('''CREATE TABLE IF NOT EXISTS run (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    level_version INTEGER,
                    player INTEGER,
                    mode TEXT,
                    duration DECIMAL,
                    recording TEXT,
                    FOREIGN KEY(level_version) REFERENCES level_version(id),
                    FOREIGN KEY(player) REFERENCES player(id)
              )''')

    g.db.commit()

@app.teardown_request
def teardown_request(exception):
    if hasattr(g, 'db'):
        g.db.close()

def save_recording(level_name, level_hash, mode, device_id, recording):
    c = g.db.cursor()
    c.execute('SELECT * FROM level WHERE name=?', (level_name,))
    level = c.fetchone()
    if level:
        level_id = level[0]
    else:
        c.execute('INSERT INTO level VALUES(NULL,?)', (level_name,))
        level_id = c.lastrowid
    
    c.execute('SELECT * FROM level_version WHERE level=? AND hash=?', (level_id,level_hash))
    level_version = c.fetchone()
    if level_version:
        level_version_id = level_version[0]
    else:
        c.execute('INSERT INTO level_version VALUES(NULL,?,?)', (level_id,level_hash))
        level_version_id = c.lastrowid

    c.execute('SELECT * FROM player WHERE unique_identifier=?', (device_id,))
    player = c.fetchone()
    if player:
        player_id = player[0]
    else:
        c.execute('INSERT INTO player VALUES(NULL,?,?,?)', (device_id, '?', '?'))
        player_id = c.lastrowid

    duration = json.loads(recording)['player recording']['total duration']
    c.execute('INSERT INTO run VALUES(NULL,?,?,?,?,?)', (level_version_id, player_id, mode, duration, recording))

    g.db.commit()


@app.route("/stats", methods=['GET'])
def stats():
    return 'Nope!'

@app.route("/submit_run", methods=['POST'])
def submit_run():
    device_id = request.form['deviceUniqueIdentifier']
    token = request.form['token']
    if token not in allowed_tokens:
        abort(401)

    level = request.form['level']
    level_hash = request.form['levelHash']
    mode = request.form['mode']

    recording = request.form['recording']

    save_recording(level, level_hash, mode, device_id, recording)

    return "OK!"

@app.route("/runs/<level_hash>", methods=['GET'])
def runs(level_hash=None):
    resp = ''
    c = g.db.cursor()

    c.execute('SELECT * FROM level_version WHERE hash = ?', (level_hash,))
    level = c.fetchone()
    if not level:
        abort(404)
    level_id = level[0]

    for run in c.execute('SELECT * FROM run WHERE level_version = ?', (level_id,)):
        resp += '%d|%d|%s in %f seconds, ' % (run[0], run[2], run[3], run[4])
    return resp[:-2]

@app.route("/run/<run_id>", methods=['GET'])
def run(run_id=None):
    resp = ''
    c = g.db.cursor()

    c.execute('SELECT * FROM run WHERE id = ?', (run_id,))
    run = c.fetchone()
    if not run:
        abort(404)

    return run[5]

@app.route("/all_runs", methods=['GET'])
def all_runs():
    resp = ''
    c = g.db.cursor()
    for level in list(c.execute('SELECT * FROM level')):
        print (level)
        resp += '<h2>%s</h2><br><ul>' % level[1]
        for version in list(c.execute('SELECT * FROM level_version WHERE level=?', (level[0],))):
            resp += '<li>Version %d</li>\n<ul>\n' % version[0]
            for player in list(c.execute('SELECT * FROM player')):
                runs = list(c.execute('SELECT * FROM run WHERE level_version=? AND player=?', (version[0],player[0])))
                if runs:
                    resp += '<li>Player %d</li>\n<ul>\n' % (player[0])
                    for run in runs:
                        rundesc = '%s in %f seconds' % (run[3], run[4])
                        resp += '<li><a href="/run/%d">%s</a></li>\n' % (run[0], rundesc)
                    resp += '</ul>'
            resp += '</ul>'
        resp += '</ul>'
    return resp



if __name__ == "__main__":
    app.run(host='0.0.0.0', port=8196, debug=True)