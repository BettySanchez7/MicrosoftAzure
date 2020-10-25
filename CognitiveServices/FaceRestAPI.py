
import json, os, requests
#Clave de suscripción
subscription_key = '3697b...'
assert subscription_key
#ENDPOINT
face_api_url = '...azure.com/' + '/face/v1.0/detect'

image_url = 'https://www.ecestaticos.com/image/clipping/0557bdb9c6166a93b7aa1e66222daecd/opening.jpg'

headers = {'Ocp-Apim-Subscription-Key': subscription_key}

#PARAMETROS
params = {
	'detectionModel': 'detection_01',
	'returnFaceAttributes': 'age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise',
  'returnFaceId': 'true'
}

response = requests.post(face_api_url, params=params,
                         headers=headers, json={"url": image_url})
print(json.dumps(response.json()))
