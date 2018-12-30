import string
import collections

# challenge is to decrypt the encrypted flag
cipher = '35P3_hfr_guvf_gb_qrpelcg_rapelcgrq_qbjaybnqf'

# knowing that the flag begins with 35C3
key = ord('P') - ord('C')
# make a list of letter
lowercase = collections.deque(string.lowercase)
# rotate the lowercase letter with the key
lowercase.rotate(key)
# make a new alphabet with the rotated list
new_alphabet = ''.join(list(lowercase))
result = ''

# decrypt the flag with caesar cipher
for char in cipher:
    # change only letters
    if char in string.ascii_letters:
        if char in string.uppercase:
            # add the new character to the result
            result += (new_alphabet[string.lowercase.index(char.lower())]).upper()
        else:
            # add the new character to the result
            result += new_alphabet[string.lowercase.index(char)]
    else:
        # add the numbers to the result
        result += char

#print the result
print(result)
