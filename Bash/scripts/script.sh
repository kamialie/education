#/bin/bash

for i in {1..15}; do
	python3 main.py > argument.txt;
	cat argument.txt >> backup.txt ;
	echo "hello" > orig_example.txt ;
	echo "world" > test_example.txt ;
	if ! diff orig_example.txt test_example.txt > /dev/null ; then
		echo "found result!" ;
		cat argument.txt
	fi
done
