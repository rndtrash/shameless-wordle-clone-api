#!/usr/bin/env python3

import sys

with open(sys.argv[1]) as fin, open(sys.argv[2], 'w') as fout:
    for line in fin.readlines():
        word = line.split(',')[0].lstrip().rstrip()
        if len(word) == 5 and word.isalpha():
            fout.write(word.lower() + '\n')