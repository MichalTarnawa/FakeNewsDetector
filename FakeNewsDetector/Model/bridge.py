#pip install flask
from flask import Flask, request, jsonify
from gradio_client import Client
import sys
import logging

log = logging.getLogger('werkzeug')
log.setLevel(logging.ERROR)

app = Flask(__name__)
client = None
current_url = ""

@app.route('/analyze', methods=['POST'])
def analyze():
    global client, current_url
    data = request.json
    url = data.get('url')
    question = data.get('question')

    try:
        if client is None or url != current_url:
            client = Client(url)
            current_url = url
        
        result = client.predict(question, api_name="/predict")
        return jsonify({"answer": result})
    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    app.run(port=5005)
#Downloading flask-3.1.3-py3-none-any.whl (103 kB)
#Downloading blinker-1.9.0-py3-none-any.whl (8.5 kB)
#Downloading itsdangerous-2.2.0-py3-none-any.whl (16 kB)
#Installing collected packages: itsdangerous, blinker, flask
#Successfully installed blinker-1.9.0 flask-3.1.3 itsdangerous-2.2.0
