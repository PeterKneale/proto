from sklearn import datasets, svm
import matplotlib.pyplot as plt

digits = datasets.load_digits()

# show digits in flattened array
x = digits.images.reshape((len(digits.images), -1))
print (x[0])
print (x[1])
print (x[2])
print (x[3])