from django.urls import path
from api import views

urlpatterns = [
    path('api/map', views.map, name='map'),
]
