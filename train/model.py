from sklearn import datasets, svm
from joblib import dump
from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import Int64TensorType

print ("Loading dataset")
dataset = datasets.load_digits()
number_samples = len(dataset.images)
data = dataset.images.reshape((number_samples, -1))

print ("Creating classifier")
model = svm.SVC(gamma=0.001)
model.fit(data, dataset.target)

print (f"Saving model (pkl)")
dump(model, 'model.pkl') 

print (f"Saving model (onnx)")
initial_type = [('image', Int64TensorType([None, 64]))]
onnx = convert_sklearn(model, initial_types=initial_type)
with open("model.onnx", "wb") as f:
    f.write(onnx.SerializeToString())