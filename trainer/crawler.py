#!/usr/bin/python
#-*- encoding: utf-8 -*-

import urllib2
import urllib
import re
import time
from htmlentitydefs import name2codepoint

def htmlentitydecode(s):
    #return re.sub('&(%s);' % '|'.join(name2codepoint), lambda m: name2codepoint[m.group(1)], s)
	return s.replace('&amp;', '&')

idx = open("index.txt", "w")
corpus = open("corpus.txt", "w")

root = "http://es.wikisource.org"
url = root + "/wiki/Categor%C3%ADa:Poes%C3%ADas"
referer = None

while True:
	print url
	
	try:
		u = urllib2.urlopen(urllib2.Request(url, None, {'Connection': 'Keep-Alive','User-Agent':'Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.861.0 Safari/535.2'}))
		
		data = u.read()
		
		u.close()
	except Exception,e:
		print e.read()
		exit(0)
	
	referer = url
	url = root + htmlentitydecode(re.findall('a href="([^"]+pagefrom[^"]+)"', data, re.DOTALL)[0])

	for x in re.findall('a href="([^"]+)"', data, re.DOTALL):
		if x.startswith('/wiki') and ':' not in x and x not in ('/wiki/Poes%C3%ADas', '/wiki/Portada'):
			poesiaurl = "%s%s" % (root, htmlentitydecode(x))
			idx.write("%s\n" % poesiaurl)
			
			print "\t",poesiaurl
			
			u = urllib2.urlopen(urllib2.Request(poesiaurl, None, {'Connection': 'Keep-Alive','User-Agent':'Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.861.0 Safari/535.2'}))
			data = u.read()
			u.close()
			
			if '<pre>' in data:
				corpus.write(re.findall('<pre>(.*?)</pre>', data, re.DOTALL)[0] + '\n')
				corpus.flush()
			elif '<div class="poem">' in data:
				corpus.write(re.sub('<[^>]+>', '', re.findall('<div class="poem">(.*?)</div>', data, re.DOTALL)[0]) + '\n')
				corpus.flush()
			elif '<p>' in data:
				for para in re.findall('<p>(.*?)</p>', data, re.DOTALL):
					corpus.write(re.sub('<[^>]+>', '', para) + '\n')
				corpus.flush()
			else:
				print data
				print 'X_X',poesiaurl
				#exit(0)
			
			time.sleep(0.5)
	
	idx.flush()
	
	if 'returnto' in url:
		break
	
	time.sleep(0.5)

idx.close()
corpus.close()