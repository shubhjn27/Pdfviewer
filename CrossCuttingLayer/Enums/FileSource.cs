namespace FileViewer.CrossCuttingLayer.Enums
{
    public enum FileSource
    {
        Core = 1,
        AWSBucket = 1<<1,
        FileStream = 1<<2
    }
}
