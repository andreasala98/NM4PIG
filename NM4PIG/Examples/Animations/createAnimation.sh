# This script uses ffmpeg
# To check if it is installed use 
# ffmpeg -version
# 
# To install use 
# brew install ffmpeg (on Mac OS X)
#
# NOTE: You may need to update your PATH

# This script on average takes ~25 minutes to be executed

#!/bin/bash

# Move to executable directory
cd ../../

# Generate 360 images
for angle in $(seq 14 359); do
    angleNNN=$(printf "%03d" $angle)
    if [ ! -f "Examples/Animations/image${angleNNN}.jpg" ]; then
       echo "Creating image with angle ${angleNNN}"
        dotnet run -- render --scene "Examples/Inputs/Other/ortho.txt" -pfm TemporaryImage.pfm -ldr Examples/Animations/image${angleNNN}.jpg -W 1080 -H 720 --declare-float angle:${angleNNN} --factor 1 &>/dev/null    
        rm -f TemporaryImage.pfm 
    fi
done

echo -e "\nI created all the images, now I proceed with creating the animation"

# Move back
cd Examples/Animations/

# Create animation
ffmpeg -r 25 -f image2 -s 640x480 -i image%03d.jpg -vcodec libx264 -pix_fmt yuv420p worldAndWikishape.mp4 &>/dev/null

# Clear useless files
# rm -f image*.jpg

echo -e "\nI created the animation in the file coloured-Spheres.mp4"

# Create also gif file
ffmpeg -i worldAndWikishape.mp4 worldAndWikishape.gif