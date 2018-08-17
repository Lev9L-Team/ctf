# Salad Upgrades (Junior, 239 solved, 20 points)

```
Sure, I could toss them all using just one shift. But am I gonna?

CIPHERTEXT: e4uo{zo1b_1e_f0j4l10i}z0ce
```

The challenge refer to the Caesar Salad challenge.
And after checking the distance of the first four characters, we know that
the key are counted up from 1 to the last character of the CIPHERTEXT.

```
key = [1,2,3,4,5,6,7, ... , 26]
```

We wrote a program, which does the rotation and solved the challenge.
The flag: 
```
d4rk{th1s_1s_r0t4t10n}c0de
```
