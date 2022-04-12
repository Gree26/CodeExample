# -*- coding: utf-8 -*-
"""
Spyder Editor

This is a temporary script file.
"""

import pandas as pd
import matplotlib.pyplot as mplt
#from scipy.optimize import cf
import tensorflow as tf
import numpy as np
import seaborn as sns
from sklearn.linear_model import LinearRegression
from sklearn.svm import SVR

df  = pd.read_csv(r'/home/username/Downloads/regression_data.csv')

x = df['X']
y = df['Y']

xR = x.values.reshape(-1,1)
yR = y.values.reshape(-1,1)

model = LinearRegression().fit(xR,yR)

#Scatter Plot
mplt.scatter(x,y,label='Regression Data')

#Linear Regression
linReg=model.predict(xR)
mplt.plot(x,linReg,label='Linear Regression', color='red')

#Polynomial Regression
polyMod = np.poly1d(np.polyfit(x, y, 3))
plotLn = np.linspace(-3, 3, 200)
mplt.plot(plotLn, polyMod(plotLn), label="Polynomial Regression",color='green')

#Show Plot
mplt.legend()

#Print The pollynomial Coefficients 
print("Polynomial Coefficients : ",polyMod.coeffs)

print("\nPOLYNOMIAL PREDICTIONS")

#Values to predict for 
predictionArray = [0,1,2,3]

for i in predictionArray:
    print(i," : ",polyMod(i))


#IGNORE THIS 
#model = tf.keras.Sequential()
#opt = tf.keras.optimizers.SGD(learning_rate=0.01, momentum=0.9)
#model.compile(optimizer='sgd', loss='binary_crossentropy', metrics=['accuracy'])
#model.fit(x,y,epochs=100,batch_size=32)
#predX = [0,1,2,3]
#newy = model.predict(predX)
#for pred in range(len(predX)):
#    print("X=%s \t\t Prediction:%s" % (predX[pred], newy[pred]))