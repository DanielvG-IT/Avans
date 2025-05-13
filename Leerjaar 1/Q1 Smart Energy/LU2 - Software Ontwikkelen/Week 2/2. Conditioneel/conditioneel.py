def find_median(a, b, c):
  if (a > b and b > c):
    return a
  elif (b > a and a > c):
    return b
  else:
    return c

def main():
  try:
    a = float(input("Enter the value of a: "))
    b = float(input("Enter the value of b: "))
    c = float(input("Enter the value of c: "))
    
    median = find_median(a, b, c)
    print(f"The median number is: {median}")
  except ValueError:
    print("Please enter valid numbers.")

if __name__ == "__main__":
  main()
