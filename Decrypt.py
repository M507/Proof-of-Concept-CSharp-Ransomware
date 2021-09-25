from Crypto.Cipher import AES
import base64, StringIO, binascii, StringIO

class PKCS7Encoder(object):
    def __init__(self, k=16):
       self.k = k
    def decode(self, text):
        '''
        Remove the PKCS#7 padding from a text string
        '''
        nl = len(text)
        val = int(binascii.hexlify(text[-1]), 16)
        if val > self.k:
            raise ValueError('Input is not padded or padding is corrupt')
        l = nl - val
        return text[:l]

# Read
file_name = "README.txt.locked"
file_in = open(file_name, 'rb')
encrypted_text = file_in.read()
# Prepare
key = bytes("kqufuwAvmcTZxQTj8x6OFNmDgisUjoi1")
IV = "PzPKZ0fuM4LIuaVa"
# Create
aes_decrypter = AES.new(key, AES.MODE_CBC, IV)
aes_decrypter.block_size = 128
# Decrypt
clear_text = PKCS7Encoder().decode(aes_decrypter.decrypt(encrypted_text))
# Save
file_name = file_name.replace(".locked","")
file_in = open(file_name, 'w')
file_in.write(clear_text)