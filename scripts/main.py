import random

orig_suits = ['s', 'h', 'c', 'd']
orig_value = ['A', 'K', 'Q', 'J', '0', '9', '8', '7', '6', '5', '4', '3', '2']

def generate_rand_suits(orig_suits):
    return [random.choice(orig_suits) for i in range(7)]
#rand_suits = [random.choice(orig_suits) for i in range(7)]

def generate_rand_sample(orig_value, rand_suits):
    return [random.choice(orig_value) + i for i in rand_suits]

def create_argument(rand_sample):
    string = ""
    for x in rand_sample:
        string += x + " "
    return string

#print(rand_suits)
#print(rand_sample)
arg_one = create_argument(generate_rand_sample(orig_value, generate_rand_suits(orig_suits)))

arg_two = create_argument(generate_rand_sample(orig_value, generate_rand_suits(orig_suits)))

#print(arg_one)
#print(arg_two)

final = arg_one + "; " + arg_two

print(final)
