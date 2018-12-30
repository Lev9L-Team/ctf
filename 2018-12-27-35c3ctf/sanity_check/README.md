# Sanity check (sanity check, 612 solved, 1 point)
###### Author: [ecurve](https://github.com/Pascalao)

```
This could help with some other challenges.

35P3_hfr_guvf_gb_qrpelcg_rapelcgrq_qbjaybnqf
````
The challenge was to decrypt the encrypted flag.
If you looked closely to the encrypted flag you can detect that the caesar cipher was used.

Mathematically (caesar cipher):
Let `x` a number, which represent a character of the alphabet and `k` the key, then the encryption are described as:

E(x) = (x+k) mod 26

And the decryption is described as:

D(x) = (x-k) mod 26

Here is another good resource of the [cipher](https://en.wikipedia.org/wiki/Caesar_cipher).

Actually we now the cipher, but we don't know the key.
But we know that the flag have to begin with 35C3 and so we compute `ord('P') - ord('C')` and get the key `13`.
From now it is easy to decrypt the flag. 

We used the program solver.py to decrypt the flag.
The flag was ```35C3_use_this_to_decrypt_encrypted_downloads```
