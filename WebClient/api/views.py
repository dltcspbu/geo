import io
import requests
import os 
import json
import random
import folium
from django.shortcuts import render
from django.http import JsonResponse
from pykalman import KalmanFilter
import numpy as np
from scipy import signal


def scipy_filter(array):

    xn = np.asarray(array)

    b, a = signal.butter(1, 1)
    y = signal.filtfilt(b, a, xn, axis=0)

    points = []
    for elem in y:
        points.append((elem[0], elem[1]))
    
    return points


def filter(array):
    measurements = np.asarray(array)
    initial_state_mean = [measurements[0, 0],
                          0,
                          measurements[0, 1],
                          0]

    transition_matrix = [[1, 1, 0, 0],
                         [0, 1, 0, 0],
                         [0, 0, 1, 1],
                         [0, 0, 0, 1]]

    observation_matrix = [[1, 0, 0, 0],
                          [0, 0, 1, 0]]

    kf1 = KalmanFilter(transition_matrices = transition_matrix,
                       observation_matrices = observation_matrix,
                       initial_state_mean = initial_state_mean)

    kf1 = kf1.em(measurements, n_iter=5)
    (smoothed_state_means, smoothed_state_covariances) = kf1.smooth(measurements)
    lats = list(smoothed_state_means[:, 0])
    lons = list(smoothed_state_means[:, 2])

    points = []
    for i in range(0, len(lats)):
        points.append((lats[i], lons[i]))
    
    return points


def map(request): 

    points = []
    times = [] 

    with open('/home/dselivanov/chainbox-thin-client/log.txt', 'r') as f: 
        for line in f: 
            array = line.split(',') 
            if len(array) > 5:
               time = array[1] 
               lat = array[2] 
               direction = array[3] 
               lon = array[4]
               if direction == 'N':
                   time_f = time[0] + time[1] + ":" + time[2] + time[3] + ":" + time[4] + time[5]
                   times.append(time_f)

                   lat_f = float(lat[0] + lat[1]) + float(lat[2] + lat[3] + "." + lat[5] + lat[6] + lat[7] + lat[8])/60
                   lat_f = float(lat_f)
                   
                   lon_f = float(lon[0] + lon[1] + lon[2]) + float(lon[3] + lon[4] + "." + lon[6] + lon[7] + lon[8] + lon[9])/60
                   lon_f = float(lon_f)
                   points.append((lat_f, lon_f))

    #points = filter(points)

    my_map = folium.Map(location=[59.939454, 30.296523], zoom_start=16, control_scale=True)
    for i in range(0, len(points)):
        folium.Marker(points[i], popup=times[i]).add_to(my_map)

    folium.PolyLine(points, color="red", weight=2.5, opacity=1).add_to(my_map)


    data = []
    for point in points:
        data.append({"latitude": point[0], "longitude": point[1]})

    #return JsonResponse(data, safe=False) 

    points_raw = requests.post('http://srv02.dltc.spbu.ru:8080/api/routes/paired?vehicle=car', json=data)
    points_json = points_raw.json()

    #return JsonResponse({"response": points_json}, safe=False)

    points_arr = []
    for point in points_json:
        points_arr.append((point["latitude"], point["longitude"]))

    folium.PolyLine(points_arr, color="green", weights=2.5, opacity=1).add_to(my_map)



    my_map.save('./static/map.html')
    return render(request, 'form.html')
