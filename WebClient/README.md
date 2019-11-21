# Установка сервера
```sh
sudo docker build -t gui .

sudo docker run --name gui \
  --restart always \
  -p 8005:8000 \
  -v <path_to_folder_with_web_client>:/usr/src/app \
  -d gui
```
