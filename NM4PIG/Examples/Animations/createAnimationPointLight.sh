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
cd ../../NM4PIG

# Generate 360 images
for angle in $(seq 0 359); do
    angleNNN=$(printf "%03d" $angle)
    echo "Creating image with angle ${angleNNN}"
    dotnet run demo -a $angle -s 7 -pfm TemporaryImage.pfm -ldr "../Examples/DemoAnimation/image${angleNNN}.jpg" &>/dev/null
    rm -f TemporaryImage.pfm
done

echo -e "\nI created all the images, now I proceed with creating the animation"

# Move back
cd ../Examples/DemoAnimation

# Create animation
ffmpeg -r 25 -f image2 -s 640x480 -i image%03d.jpg -vcodec libx264 -pix_fmt yuv420p pointLight.mp4 &>/dev/null

# Clear useless files
rm -f image*.jpg

echo -e "\nI created the animation in the file pointLight.mp4"

# Create also gif file
ffmpeg -i pointLight.mp4 pointLight.gif
