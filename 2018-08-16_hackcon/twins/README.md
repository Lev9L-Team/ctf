# Twins (Steg, 239 solved, 20 points)
###### Author: [ecurve](https://github.com/Pascalao)

```
Two brothers separated long ago, finally decide to reunite

Each one holds the secret key, but needs the other brother to read it.

Can you find their hidden secret?
```

[file1](https://github.com/Lev9L-Team/ctf/tree/master/2018-08-16_hackcon/twins/file1)
[file2](https://github.com/Lev9L-Team/ctf/tree/master/2018-08-16_hackcon/twins/file2)

We asked us, which are common by twins?
They have the same DNA / DNS. 

So we build a tool, which checks the characters in the file, if they are the same in the same position
and print all those characters.
And we get the flag!

```
d4rk{lo0king_p4st_0ur_d1ff3renc3s}c0de
```

The program is in the file [twins.py](https://github.com/Lev9L-Team/ctf/tree/master/2018-08-16_hackcon/twins/twins.py)

