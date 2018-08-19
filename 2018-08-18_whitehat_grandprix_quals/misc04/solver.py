#!/usr/bin/env python2

from pwn import *
import re


def totient(n):          # n - unsigned int
    result = 1
    p = 2                 #prime numbers - 'iterator'
    while p**2 <= n:
        if n%p == 0:    # * (p-1)
            result *= (p-1)
            n /= p
        while n % p == 0: # * p^(k-1)
            result *= p
            n /= p
        p += 1
    if n != 1:
        result *= (n-1)
    return result


def modpow(p, z, b, c, m): # (p^z)^(b^c) mod m
    if m == 2:
        return p % m
    cp = 0
    while m % p == 0:
        cp += 1
        m /= p              # m = m' now
    t = totient(m)
    exponent = ((pow(b,c,t)*z) % t + t - (cp % t)) % t
                            # exponent = z*(b^c)-cp mod t
    return pow(p, cp)*pow(p, exponent, m)


def solve(a,b,c,m): #split
    result = 1
    p = 2
    while p**2 <= a:
        cp = 0
        while a%p == 0:
            a /= p
            cp += 1
        if cp != 0:
           temp = modpow(p,cp,b,c,m)
           result *= temp
           result %= m
        p += 1
    if a != 1:
        result *= modpow(a, 1, b, c, m)
    return result % m


def solving(r):

    data1 = r.recvuntil('Face_index:' , timeout=500)
    face_index = r.recvline(timeout=500)
    face_index = int(face_index.split(' ')[1].replace('\n', ''))
    print data1 + str(face_index)
    # print 'face_index:' + str(face_index)

    data2 = r.recvline(timeout=500)
    print data2

    faces = r.recvlines(134, timeout=500)
    for data in faces:
        print data
    res2 = []
    res3 = []

    for data in faces:
        data = re.sub(' +', ' ', data).split(' ')
        lip = int(data[1])
        nose = int(data[2])
        eyes = int(data[3])
        forehead = int(data[4])
        res = solve(pow(lip, nose, face_index), eyes, forehead, face_index)
        res2.append(res)
        res3.append((res, data[0]))

    print r.recvline() # So, waht is the most friendly face
    res3.sort(key=operator.itemgetter(0), reverse=True)

    print res3[0]

    r.sendline(str(res3[0][1]))
    r.sendline(str(res3[0][0]))


def main():
    r = remote('misc04.grandprix.whitehatvn.com', 1337)
    while True:
        try:
            solving(r)
        except EOFError:
            print r.recvall(timeout=200)
            break
    r.close()


if __name__ == '__main__':
    main()
