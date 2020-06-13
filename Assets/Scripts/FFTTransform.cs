using CSCore.Utils;
using System;
using CSCore.DSP;

/// <summary>Provides FFT calculations.</summary>
/// <remarks>
/// Usage: Use the <see cref="M:CSCore.DSP.FftProvider.Add(System.Single[],System.Int32)" />-method to input samples to the <see cref="T:CSCore.DSP.FftProvider" />. Use the <see cref="M:CSCore.DSP.FftProvider.GetFftData(System.Single[])" /> method to
/// calculate the Fast Fourier Transform.
/// </remarks>
public class FftTransform
{
  private readonly int _channels;
  private readonly int _fftSize;
  private readonly int _fftSizeExponent;
  private readonly Complex[] _storedSamples;
  private int _currentSampleOffset;
  private volatile bool _newDataAvailable;

  /// <summary>Gets the specified fft size.</summary>
  public int FftSize
  {
    get
    {
      return this._fftSize;
    }
  }

  /// <summary>
  /// Gets a value which indicates whether new data is available.
  /// </summary>
  public virtual bool IsNewDataAvailable
  {
    get
    {
      return this._newDataAvailable;
    }
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="T:CSCore.DSP.FftProvider" /> class.
  /// </summary>
  /// <param name="channels">Number of channels of the input data.</param>
  /// <param name="fftSize">The number of bands to use.</param>
  /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="channels" /> is less than zero.</exception>
  public FftTransform(int channels, int fftSize)
  {
    if (channels < 1)
      throw new ArgumentOutOfRangeException(nameof (channels));
    double num = Math.Log((double) fftSize, 2.0);
    if (num % 1.0 != 0.0 || num == 0.0)
      throw new ArgumentOutOfRangeException(nameof (fftSize));
    this._channels = channels;
    this._fftSize = fftSize;
    this._fftSizeExponent = (int) num;
    this._storedSamples = new Complex[(int) fftSize];
  }

  /// <summary>
  /// Adds a <paramref name="left" /> and a <paramref name="right" /> sample to the <see cref="T:CSCore.DSP.FftProvider" />. The <paramref name="left" /> and the <paramref name="right" /> sample will be merged together.
  /// </summary>
  /// <param name="left">The sample of the left channel.</param>
  /// <param name="right">The sample of the right channel.</param>
  public virtual void Add(float left, float right)
  {
    this._storedSamples[this._currentSampleOffset].Imaginary = 0.0f;
    this._storedSamples[this._currentSampleOffset].Real = (float) (((double) left + (double) right) / 2.0);
    ++this._currentSampleOffset;
    if (this._currentSampleOffset >= this._storedSamples.Length)
      this._currentSampleOffset = 0;
    this._newDataAvailable = true;
  }

  /// <summary>
  /// Adds multiple samples to the <see cref="T:CSCore.DSP.FftProvider" />.
  /// </summary>
  /// <param name="samples">Float Array which contains samples.</param>
  /// <param name="count">Number of samples to add to the <see cref="T:CSCore.DSP.FftProvider" />.</param>
  public virtual void Add(float[] samples, int count)
  {
    if (samples == null)
      throw new ArgumentNullException(nameof (samples));
    count -= count % this._channels;
    if (count > samples.Length)
      throw new ArgumentOutOfRangeException(nameof (count));
    int num = count / this._channels;
    for (int i = 0; i < num; i += this._channels)
    {
      this._storedSamples[this._currentSampleOffset].Imaginary = 0.0f;
      this._storedSamples[this._currentSampleOffset].Real = this.MergeSamples(samples, i, this._channels);
      ++this._currentSampleOffset;
      if (this._currentSampleOffset >= this._storedSamples.Length)
        this._currentSampleOffset = 0;
    }
    this._newDataAvailable = count > 0;
  }

  /// <summary>
  /// Calculates the Fast Fourier Transform and stores the result in the <paramref name="fftResultBuffer" />.
  /// </summary>
  /// <param name="fftResultBuffer">The output buffer.</param>
  /// <returns>Returns a value which indicates whether the Fast Fourier Transform got calculated. If there have not been added any new samples since the last transform, the FFT won't be calculated. True means that the Fast Fourier Transform got calculated.</returns>
  public virtual bool GetFftData(Complex[] fftResultBuffer)
  {
    if (fftResultBuffer == null)
      throw new ArgumentNullException(nameof (fftResultBuffer));
    Complex[] data = fftResultBuffer;
    Array.Copy((Array) this._storedSamples, this._currentSampleOffset, (Array) data, 0, this._storedSamples.Length - this._currentSampleOffset);
    Array.Copy((Array) this._storedSamples, 0, (Array) data, this._storedSamples.Length - this._currentSampleOffset, this._currentSampleOffset);
    FastFourierTransformation.Fft(data, this._fftSizeExponent, FftMode.Forward);
    bool newDataAvailable = this._newDataAvailable;
    this._newDataAvailable = false;
    return newDataAvailable;
  }

  /// <summary>
  /// Calculates the Fast Fourier Transform and stores the result in the <paramref name="fftResultBuffer" />.
  /// </summary>
  /// <param name="fftResultBuffer">The output buffer.</param>
  /// <returns>Returns a value which indicates whether the Fast Fourier Transform got calculated. If there have not been added any new samples since the last transform, the FFT won't be calculated. True means that the Fast Fourier Transform got calculated.</returns>
  public virtual bool GetFftData(float[] fftResultBuffer)
  {
    if (fftResultBuffer == null)
      throw new ArgumentNullException(nameof (fftResultBuffer));
    if (fftResultBuffer.Length < this._fftSize)
      throw new ArgumentException("Length of array must be at least as long as the specified fft size.", nameof (fftResultBuffer));
    Complex[] fftResultBuffer1 = new Complex[(int) this._fftSize];
    bool newDataAvailable = this._newDataAvailable;
    this.GetFftData(fftResultBuffer1);
    for (int index = 0; index < fftResultBuffer1.Length; ++index)
      fftResultBuffer[index] = (float) fftResultBuffer1[index];
    return newDataAvailable;
  }

  private float MergeSamples(float[] samples, int i, int channels)
  {
    switch (channels)
    {
      case 1:
        return samples[i];
      case 2:
        return (float) (((double) samples[i] + (double) samples[i + 1]) / 2.0);
      case 3:
        return (float) (((double) samples[i] + (double) samples[i + 1] + (double) samples[i + 2]) / 3.0);
      case 4:
        return (float) (((double) samples[i] + (double) samples[i + 1] + (double) samples[i + 2] + (double) samples[i + 3]) / 4.0);
      case 5:
        return (float) (((double) samples[i] + (double) samples[i + 1] + (double) samples[i + 2] + (double) samples[i + 3] + (double) samples[i + 4]) / 5.0);
      case 6:
        return (float) (((double) samples[i] + (double) samples[i + 1] + (double) samples[i + 2] + (double) samples[i + 3] + (double) samples[i + 4] + (double) samples[i + 5]) / 6.0);
      default:
        float num1 = 0.0f;
        int num2;
        for (int index1 = i; index1 < channels; index1 = num2 + 1)
        {
          double num3 = (double) num1;
          float[] numArray = samples;
          int index2 = index1;
          num2 = index2 + 1;
          double num4 = (double) numArray[index2];
          num1 = (float) (num3 + num4);
        }
        return num1 / (float) channels;
    }
  }
}

