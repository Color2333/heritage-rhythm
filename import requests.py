import requests

url = " https://burn.hair/v1/chat/completions"
payload = {
    "model": "gpt-3.5-turbo",
    "messages": [
        {"role": "system", "content": "你是一个聊天机器人，请回答我"},
        {"role": "user", "content": "今天天气咋样？"}
    ]
}
headers = {
    "Authorization": "Bearer sk-5cEKSH8D75jFJlDWC46b222b2bB440Ec98F477AfC4Bf9202",
    "Content-Type": "application/json"
}

response = requests.post(url, json=payload, headers=headers)

print(response.text)
