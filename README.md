# AWS Lambda Image Converter project
ImageConverterFunction is a simple pet project that demonstrates how to use AWS Lambda and the SixLabors.ImageSharp package to convert JPG images to GIF, PNG, and BMP formats. The project is written in C# and can be easily modified to support other image formats or additional functionality.

To use the JpgConverter, you will need to have an AWS account and create a new Lambda function using the C# runtime. Once the function is set up, you can upload the JpgConverter code and configure the function to trigger when a new image is added to an S3 bucket.

The JpgConverter function will automatically convert the new image to the specified format and save it to a separate S3 bucket with the same file name. You can customize the output file name and image quality by modifying the code.

To get started with the JpgConverter, follow these steps:

1. Set up an AWS account and new Lambda function.

2. Configure function to trigger when a new image is added to a specific S3 bucket.

3. Download the JpgConverter code.

4. Modify the code to customize the output file name and image converting properties as needed.

5. Build with the next command and zip it.
``` dotnet publish -c Release -o ./publish -r linux-x64 --self-contained true ```

6. Deploy the functions and enjoy the convenience of automated image conversion using AWS Lambda and C#.

Test the functions by uploading an image to the S3 bucket and verifying that it is converted to the .gif, .png, and .bmp formats and saved to the corresponding S3 buckets.