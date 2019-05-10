import re

file = input("Enter file name: ")
if file == '':
    file = "regex_sum_42.txt"

try:
    fh = open(file)
except:
    print("Could not open file", file)
    quit()

print(sum([int(n) for n in re.findall('\d+', fh.read())]))