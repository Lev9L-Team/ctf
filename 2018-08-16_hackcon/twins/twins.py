with open('file1','r') as f:
    file1 = f.read()

with open('file2','r') as f:
    file2 = f.read()


result = ''
for f1, f2 in zip(file1, file2):
    if f1 == f2:
        result += f1

print(result)
