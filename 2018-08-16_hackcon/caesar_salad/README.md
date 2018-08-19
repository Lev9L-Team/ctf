# Caesar Salad (Junior, 413 solved, 10 points)
###### Author: @pascalao (ecurve)

```
Can you toss Caesar's salad?

q4ex{t1g_thq_p4rf4e}p0qr
```

We know that the flag have the format of d4rk{<flag>}c0de. 
We compute the distance from d to q with:

```python
abs(ord('d')-ord('q'))
```

And we get the distance of 13.
The key of the caeser chiffre are 13.

We rotate the characters and get the flag d4rk{g1t_gud_c4es4r}c0de.

The code to decipher this ciphertext are in [exploit.py](https://github.com/Lev9L-Team/ctf/tree/master/2018-08-16_hackcon/caesar_salad/exploit.py)