import re

file = input("Enter file name: ")
if file == '':
    file = "regex_sum_42.txt"

try:
    fh = open(file)
except:
    print("Could not open file", file)
    quit()

numbers = list()
for line in fh:
    lst = re.findall("\d+", line)
    if (len(lst)) != 0:
        numbers.extend(lst)

total_sum = 0
for n in numbers:
    total_sum += int(n)
print(total_sum)