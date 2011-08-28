#!/usr/bin/python

import struct
import random
import time

random.seed(time.time())

def getword(f, idx):
	f.seek(2 + 3 * idx)
	s = '\0' + f.read(3)
	
	pos = struct.unpack('!L', s)[0]
	
	f.seek(pos)
	
	l,nextlen = struct.unpack('!BH', f.read(3))
	
	return f.read(l),nextlen,pos+3+l

f = open('markov.bin', 'rb')

wordcount = struct.unpack('!H', f.read(2))[0]

wordidx = random.randint(0, wordcount)

for i in xrange(12):
	if wordidx < 0: break
	
	word,nextlen,pos = getword(f, wordidx)
	print word,
	
	wordidx = random.randint(0, nextlen-1)
	
	f.seek(pos + wordidx * 2)
	
	wordidx = struct.unpack('!H', f.read(2))[0] - 1

f.close()