for f in *.png
do
if [[ $f != *"G.png" ]]; then
	echo "Removing $f"
	rm $f
fi
done

for f in *.jpg
do
if [[ $f != *"G.jpg" ]]; then
	echo "Removing $f"
	rm $f
fi
done


for f in *.pfm
do
if [[ $f == "demoImage.pfm" ]]; then
	echo "Removing $f"
	rm $f
fi
done


