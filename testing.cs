using System;
using FFmpeg.AutoGen;
using FFmpegWrapper;

namespace AudioUpscale
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            string inputFile = @"C:\input.mp3";
            string outputFile = @"C:\output.flac";

            FFmpegBinariesHelper.RegisterFFmpegBinaries();

            ffmpeg.av_register_all();
            ffmpeg.avformat_network_init();

            var inputFormatContext = ffmpeg.avformat_alloc_context();

            if (ffmpeg.avformat_open_input(&inputFormatContext, inputFile, null, null) != 0)
            {
                Console.WriteLine($"Error occurred while opening file '{inputFile}'");
                return;
            }

            if (ffmpeg.avformat_find_stream_info(inputFormatContext, null) < 0)
            {
                Console.WriteLine($"Error occurred while getting stream info");
                return;
            }

            int audioStreamIndex = -1;

            for (int i = 0; i < inputFormatContext->nb_streams; i++)
            {
                if (inputFormatContext->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
                {
                    audioStreamIndex = i;
                    break;
                }
            }

            if (audioStreamIndex == -1)
            {
                Console.WriteLine($"Could not find audio stream in the input file");
                return;
            }

            var sourceCodecContext = *inputFormatContext->streams[audioStreamIndex]->codec;

            var targetCodec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_FLAC);

            if (targetCodec == null)
            {
                Console.WriteLine($"Target codec '{AVCodecID.AV_CODEC_ID_FLAC}' not found");
                return;
            }

            var targetCodecContext = ffmpeg.avcodec_alloc_context3(targetCodec);

            if (!targetCodec->capabilities.HasFlag(AVCodecCapabilities.AV_CODEC_CAP_VARIABLE_FRAME_SIZE))
            {
                Console.WriteLine($"Target codec '{targetCodec->name}' does not support variable frame sizes");
                return;
            }

            ffmpeg.avcodec_parameters_to_context(targetCodecContext, inputFormatContext->streams[audioStreamIndex]->codecpar);

            targetCodecContext->sample_fmt = targetCodec->sample_fmts[0];
            targetCodecContext->bit_rate = 320000;
            targetCodecContext->sample_rate = sourceCodecContext.sample_rate;
            targetCodecContext->channels = sourceCodecContext.channels;
            targetCodecContext->channel_layout = sourceCodecContext.channel_layout;

            if (ffmpeg.avcodec_open2(targetCodecContext, targetCodec, null) < 0)
            {
                Console.WriteLine($"Could not open target codec '{targetCodec->name}'");
                return;
            }

            var targetFrame = ffmpeg.av_frame_alloc();

            if (targetFrame == null)
            {
                Console.WriteLine($"Could not allocate target frame");
                return;
            }

            targetFrame->format = (int)targetCodecContext->sample_fmt;
            targetFrame->channel_layout = targetCodecContext->channel_layout;
            targetFrame->sample_rate = targetCodecContext->sample_rate;
            targetFrame->nb_samples = ffmpeg.av_rescale_rnd(sourceCodecContext.frame_size, targetCodecContext->sample_rate, sourceCodecContext.sample_rate, AVRounding.AV_ROUND_UP);

            if (ffmpeg.av_frame_get_buffer(targetFrame, 0) < 0)
            {
                Console.WriteLine($"Could not allocate target data buffers");
                return;
            }

            var packet = ffmpeg.av_packet_alloc();

            if (packet == null)
            {
                Console.WriteLine($"Could not allocate packet");
                return;
            }

            while (ffmpeg.av_read_frame(inputFormatContext, packet) >= 0)
            {
                if (packet->stream_index != audioStreamIndex)
                {
                    ffmpeg.av_packet_unref(packet);
                    continue;
                }

                var inputFrame = ffmpeg.av_frame_alloc();

                if (inputFrame == null)
                {
                    Console.WriteLine($"Could not allocate input frame");
                    break;
                }

                int consumedBytes = 0;

                int error = ffmpeg.avcodec_send_packet(&sourceCodecContext, packet);

                if (error < 0)
                {
                    Console.WriteLine($"Error sending a packet for decoding - {error}");
                    break;
                }

                while (true)
                {
                    error = ffmpeg.avcodec_receive_frame(&sourceCodecContext, inputFrame);

                    if (error == AVERROR.AVERROR_EAGAIN || error == AVERROR.AVERROR_EOF)
                    {
                        ffmpeg.av_frame_unref(inputFrame);
                        break;
                    }

                    if (error < 0)
                    {
                        Console.WriteLine($"Error while decoding - {error}");
                        return;
                    }

                    var scaleFrame = ffmpeg.av_frame_alloc();

                    if (scaleFrame == null)
                    {
                        Console.WriteLine($"Could not allocate scale frame");
                        break;
                    }

                    scaleFrame->format = (int)targetCodecContext->sample_fmt;
                    scaleFrame->channel_layout = targetCodecContext->channel_layout;
                    scaleFrame->sample_rate = targetCodecContext->sample_rate;

                    error = ffmpeg.swr_convert_frame(null, scaleFrame, inputFrame);

                    if (error < 0)
                    {
                        Console.WriteLine($"Error while audio resampling - {error}");
                        break;
                    }

                    error = ffmpeg.avcodec_send_frame(targetCodecContext, scaleFrame);

                    if (error == AVERROR.AVERROR_EOF)
                    {
                        ffmpeg.av_frame_free(&scaleFrame);
                        break;
                    }

                    if (error < 0)
                    {
                        Console.WriteLine($"Error sending a frame for encoding - {error}");
                        break;
                    }

                    while (true)
                    {
                        error = ffmpeg.avcodec_receive_packet(targetCodecContext, packet);

                        if (error == AVERROR.AVERROR_EAGAIN || error == AVERROR.AVERROR_EOF)
                        {
                            break;
                        }

                        if (error < 0)
                        {
                            Console.WriteLine($"Error while encoding - {error}");
                            break;
                        }

                        int bytesWritten;

                        error = ffmpeg.av_interleaved_write_frame(inputFormatContext, packet);

                        if (error < 0)
                        {
                            Console.WriteLine($"Error while writing output - {error}");
                            break;
                        }

                        ffmpeg.av_packet_unref(packet);
                    }

                    ffmpeg.av_frame_unref(scaleFrame);
                }

                ffmpeg.av_frame_free(&inputFrame);
            }

            ffmpeg.av_write_trailer(inputFormatContext);

            ffmpeg.avcodec_free_context(&targetCodecContext);

            ffmpeg.avformat_close_input(&inputFormatContext);

            var outputFileInfo = new System.IO.FileInfo(outputFile);

            if (outputFileInfo.Exists)
            {
                outputFileInfo.Delete();
            }

            System.IO.File.Move(@"C:\output.flac.tmp", outputFile);
        }
    }
}
