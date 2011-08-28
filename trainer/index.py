#!/usr/bin/python

import re
import sys
import struct

para = ""

for line in open("corpus.txt", "r"):
	words = re.sub('\\d+', '', line)\
		.lower()\
		.replace('Á', 'á')\
		.replace('É', 'é')\
		.replace('Í', 'í')\
		.replace('Ó', 'ó')\
		.replace('Ú', 'ú')\
		.replace('Ñ', 'ñ')\
		.strip()\
		.split(" ") 
	
	if len(words) <3:
		words = []
		line = ""
	
	para += " " + " ".join(words)

vocab = {}
freq = {}

for line in re.sub("([^.])([.?])([^.])", "\\1\\2\n\\3", para).split("\n"):
	line = line.strip()
	if len(line.split(" ")) <= 3: continue
	line = filter(lambda x: x, map(lambda x: x.strip(), re.split("([ .,:?]+)", line))) + [None]
	
	prev = None
	for word in line:
		if word:
			if ' ' in word:
				continue
			
			if word not in freq:
				freq[word] = 0
			freq[word] += 1
		
		if prev not in vocab:
			vocab[prev] = {}
		if word not in vocab[prev]:
			vocab[prev][word] = 1
		vocab[prev][word] += 1
		
		prev = word

size = 2 + 3 * len(freq)
maxl = 0
maxll = 0

f = open("markov.bin", "wb")

keys = freq.keys()
indexes = {}
buffer = ""

f.write(struct.pack('!H', len(keys)))

for key in keys:
	pos = len(indexes)
	indexes[key] = pos

pos = 2 + 3 * len(keys)

for key in keys:
	f.write(struct.pack('!L', pos)[1:])
	
	entry = struct.pack('!BH', len(key), len(vocab[key])) + key
	
	for next in vocab[key]:
		if not next:
			entry += struct.pack('!H', 0)
		else:
			entry += struct.pack('!H', indexes[next] + 1)
	
	pos += len(entry)
	buffer += entry

f.write(buffer)

f.close()

"""
for elm in sorted([(freq[x],x) for x in freq]):
	sys.stdout.write(elm[1])
"""