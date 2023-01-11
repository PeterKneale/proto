import numpy as np 
import matplotlib.pyplot as plt

from sklearn import datasets

digits = datasets.load_digits()

# show digits in graphically
plt.imshow(digits.images[0], cmap=plt.cm.gray_r,interpolation='nearest')
plt.show()
plt.imshow(digits.images[1], cmap=plt.cm.gray_r,interpolation='nearest')
plt.show()
plt.imshow(digits.images[2], cmap=plt.cm.gray_r,interpolation='nearest')
plt.show()
plt.imshow(digits.images[3], cmap=plt.cm.gray_r,interpolation='nearest')
plt.show()

# show digits in flattened array
x = digits.images.reshape((len(digits.images), -1))
print (x[0])
print (x[1])
print (x[2])
print (x[3])