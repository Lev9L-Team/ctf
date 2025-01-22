# Confusion

For this challenge the following source code was provided: 

```Python
#!/usr/bin/env python3

from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad
import os

# Local imports
FLAG = os.getenv("FLAG", "srdnlen{REDACTED}").encode()

# Server encryption function
def encrypt(msg, key):
    pad_msg = pad(msg, 16)
    blocks = [os.urandom(16)] + [pad_msg[i:i + 16] for i in range(0, len(pad_msg), 16)]

    b = [blocks[0]]
    for i in range(len(blocks) - 1):
        tmp = AES.new(key, AES.MODE_ECB).encrypt(blocks[i + 1])
        b += [bytes(j ^ k for j, k in zip(tmp, blocks[i]))]

    c = [blocks[0]]
    for i in range(len(blocks) - 1):
        c += [AES.new(key, AES.MODE_ECB).decrypt(b[i + 1])]

    ct = [blocks[0]]
    for i in range(len(blocks) - 1):
        tmp = AES.new(key, AES.MODE_ECB).encrypt(c[i + 1])
        ct += [bytes(j ^ k for j, k in zip(tmp, c[i]))]

    return b"".join(ct)


KEY = os.urandom(32)

print("Let's try to make it confusing")
flag = encrypt(FLAG, KEY).hex()
print(f"|\n|    flag = {flag}")

while True:
    print("|\n|  ~ Want to encrypt something?")
    msg = bytes.fromhex(input("|\n|    > (hex) "))

    plaintext = pad(msg + FLAG, 16)
    ciphertext = encrypt(plaintext, KEY)

    print("|\n|  ~ Here is your encryption:")
    print(f"|\n|   {ciphertext.hex()}")

```
Observe that the flag is given as a ciphertext by
```Python
flag = encrypt(FLAG, KEY).hex()
print(f"|\n|    flag = {flag}")
```
Let the ciphertext of the flag be $ct^{\mathop{flag}}$.


Let $x$ be a plaintext and $ct$ the corresponding ciphertext that is given by the encryption function above.
Further let $i \in \mathbb{N}$ be an index and let denote $\oplus$ the xor function of one block.
Observe that the encryption function is given by 

$$ct_{i+1} = E(D(E(x_{i+1}) \oplus x_i)) \oplus D(E(x_i) \oplus x_{i-1}),$$

where $E$ is the encryption of an block in ECB-mode and $D$ the decryption of an block in ECB-mode.
By the fact that the decryption $D$ is the inverse function of $E$ this equation can be simplified to
$$
ct_{i+1} = E(x_{i+1}) \oplus x_i \oplus D(E(x_i) \oplus x_{i-1}).
$$

Observe further that $ct_0 = x_0$ and $ct_1 = E(x_1)$.

The encryption oracle
```Python
while True:
    print("|\n|  ~ Want to encrypt something?")
    msg = bytes.fromhex(input("|\n|    > (hex) "))

    plaintext = pad(msg + FLAG, 16)
    ciphertext = encrypt(plaintext, KEY)

    print("|\n|  ~ Here is your encryption:")
    print(f"|\n|   {ciphertext.hex()}")
```
is providing the opportunity to encrypt any plaintext with the encryption function with an unknown key.
By observing that $ct_1 = E(x_1)$ it is possible to encrypt any plaintext block with this unknown key if we put it into the first block of the plaintext.
Any further block will be encrypted by the more "complex" formula above.
This provides a pure encryption function that may be used later.
By analyzing 

$$ct_3 =  E(x_3) \oplus x_2 \oplus D(E(x_2) \oplus x_1)$$

Observe that using the plaintext blocks $x_1$ and $x_2$ it is possible to manipulate this equation.


Let now $x_2 = x_3 = 0$. 
Using the encryption oracle for one call it is possible to get $E(0) = ct_1 = y$.
As a reminder $x_2, x_3, x_1$ can be chosen arbitrary, i.e. they are controlled by the adversary, and $E(x_1)$ is known.
Let $x_1 = x' \oplus E(x_2)$.
Thus, it follows

$$ct_3 \oplus E(x_3) \oplus x_2 = D(E(x_2) \oplus x_1) = D(E(x_2) \oplus x_1) = D(E(x_2) \oplus x' \oplus E(x_2)) = D(x').$$

This provides an decryption oracle using the encryption function above.
Let give this a name $dec_{\mathop{oracle}}(x')$.
Thus, it is now possible to decrypt the first block of the ciphertext via $dec_{\mathop{oracle}}(x_1^{\mathop{flag}})$.

To decrypt the other ciphertext blocks of the flag it is necessary to retrieve the pur encryption of the flag, i.e. $E(x_i^{\mathop{flag}})$ where $i \in \mathbb{N}$ is the index of the particular ciphertext block of the encrypted flag.
Since $x_1^{\mathop{flag}}, x_2^{\mathop{flag}}$ and $E(x_2^{\mathop{flag}})$ is known it follows that

$$ct_i^{\mathop{flag}} \oplus x_{i-1}^{\mathop{flag}} \oplus D(E(x_{i-1}^{\mathop{flag}}) \oplus x_{i-2}^{\mathop{flag}}) = E(x_i^{\mathop{flag}}),$$

where $i geq 2$.
Thus, the the plaintext blocks of the encrypted flag is given by

$$dec_{\mathop{oracle}}(ct_i^{\mathop{flag}}) = x_i^{\mathop{flag}}.$$

Thus, the followin script is providing the flag:

```Python
from pwn import *
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad


def xor(a, b):
    return bytes(x ^ y for x, y in zip(a, b))


def encrypt(r, msg):
    r.sendlineafter(b"> (hex) ", msg.hex().encode())  # Send input as hex
    r.recvuntil(b"Here is your encryption:\n|\n|   ")
    return bytes.fromhex(r.recvline().strip().decode())

def enc_oracle(r, msg):
    return encrypt(r, msg)[AES.block_size:2*AES.block_size]

def dec_oracle(r, msg, j = 3):
    zero = enc_oracle(r, b"\x00"*AES.block_size)
    x2 = b"\x00" * AES.block_size
    x3 = b"\x00" * AES.block_size
    pt = xor(msg, zero) + x2 + x3

    ct = encrypt(r, pt)[j*AES.block_size:(j+1)*AES.block_size]
    plaintext = xor(ct, zero)

    return plaintext

def decrypt(r, x, y, ct):
    enc_x = enc_oracle(r, x)
    dec = dec_oracle(r, xor(enc_x, y))
    return dec_oracle(r, xor(xor(dec, x), ct))

def main():
    local = True
    host = "confusion.challs.srdnlen.it"
    port = 1338
    recovered_flag = b""
    if local:
        r = process(['./chall.py'])
    else:
        r = remote(host, port)

    info(r.recvuntil("flag = "))
    flag = bytes.fromhex(r.recvline().strip(b"\n").decode())
    info(f"flag: {flag}")

    f1 = dec_oracle(r, flag[1*AES.block_size:2*AES.block_size])
    print(f"f1: {f1}")
    recovered_flag += f1
    flag_parts = [flag[0:AES.block_size], f1]

    print(f"len: {len(flag)//AES.block_size}")
    for i in range(len(flag)//AES.block_size -2):
        f = decrypt(r, flag_parts[i+1], flag_parts[i], flag[(i+2)*AES.block_size: (i+3)*AES.block_size])
        flag_parts.append(f)
        recovered_flag += f

    print(f"recovered_flag: {unpad(recovered_flag, AES.block_size)}")


if __name__ == "__main__":
    main()

```
---------------
# Alternative solution

An alternative solution would be to exploit the structure of encryption oracle since it concatenates the flag after the message such that it would be enough to brute force every character until the encryption is the same as the correct plaintext would be known.
However, I thought this solution would be more intersting since it is giving even a full decryption routine.