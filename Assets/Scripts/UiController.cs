using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI unityDeviceModelText;
	[SerializeField] TextMeshProUGUI unityCpuCoresText;
	[SerializeField] TextMeshProUGUI unityOsVersionText;
	[SerializeField] TextMeshProUGUI unityTotalMemoryText;
	[SerializeField] TextMeshProUGUI unityDeviceVendorText;

	[SerializeField] TextMeshProUGUI nativeDeviceModelText;
	[SerializeField] TextMeshProUGUI nativeCpuCoresText;
	[SerializeField] TextMeshProUGUI nativeOsVersionText;
	[SerializeField] TextMeshProUGUI nativeTotalMemoryText;
	[SerializeField] TextMeshProUGUI nativeDeviceVendorText;

	[SerializeField] TextMeshProUGUI deviceModelButtonText;
	[SerializeField] TextMeshProUGUI cpuCoresButtonText;
	[SerializeField] TextMeshProUGUI osVersionButtonText;
	[SerializeField] TextMeshProUGUI totalMemoryButtonText;
	[SerializeField] TextMeshProUGUI deviceVendorButtonText;

	[SerializeField] TextMeshProUGUI getAllInfoButtonText;
	[SerializeField] TextMeshProUGUI countdownText;
	[SerializeField] TextMeshProUGUI clearButtonText;

	[SerializeField] Button deviceModelButton;
	[SerializeField] Button cpuCoresButton;
	[SerializeField] Button osVersionButton;
	[SerializeField] Button totalMemoryButton;
	[SerializeField] Button deviceVendorButton;

	[SerializeField] Button getAllInfoButton;
	[SerializeField] Button clearButton;

	IDeviceInfo _unityDeviceInfo;
	IDeviceInfo _nativeDeviceInfo;

	CancellationTokenSource _tokenDeviceModel;
	CancellationTokenSource _tokenCpuCores;
	CancellationTokenSource _tokenOsVersion;
	CancellationTokenSource _tokenDeviceMemory;
	CancellationTokenSource _tokenDeviceVendor;

	CancellationTokenSource _tokenClearText;

	private List<CancellationTokenSource> _activeTokens;

	bool _deviceModelWasPressed;
	bool _cpuCoresWasPressed;
	bool _osVersionWasPressed;
	bool _totalMemoryWasPressed;
	bool _deviceVendorWasPressed;
	
	bool _clearTextWasPressed;
	

	void Start()
	{
		ClearAllText();

		deviceModelButton.onClick.AddListener(ToggleDeviceModel);
		cpuCoresButton.onClick.AddListener(ToggleCpuCores);
		osVersionButton.onClick.AddListener(ToggleOsVersion);
		totalMemoryButton.onClick.AddListener(ToggleTotalMemory);
		deviceVendorButton.onClick.AddListener(ToggleDeviceVendor);

		getAllInfoButton.onClick.AddListener(GetAllDeviceInfo);
		clearButton.onClick.AddListener(ClearAfterDelay);

		_activeTokens = new List<CancellationTokenSource>();

		_unityDeviceInfo = new EditorDeviceInfo();

#if UNITY_ANDROID && !UNITY_EDITOR
        _nativeDeviceInfo = new AndroidDeviceInfo();
#elif UNITY_IOS && !UNITY_EDITOR
        _nativeDeviceInfo = new AppleDeviceInfo();
#endif
	}

	void ClearAllText()
	{
		unityDeviceModelText.text = string.Empty;
		unityCpuCoresText.text = string.Empty;
		unityOsVersionText.text = string.Empty;
		unityTotalMemoryText.text = string.Empty;
		unityDeviceVendorText.text = string.Empty;

		nativeDeviceModelText.text = string.Empty;
		nativeCpuCoresText.text = string.Empty;
		nativeOsVersionText.text = string.Empty;
		nativeTotalMemoryText.text = string.Empty;
		nativeDeviceVendorText.text = string.Empty;

		countdownText.text = string.Empty;
	}

	void GetAllDeviceInfo()
	{
			if (!_deviceModelWasPressed) ToggleDeviceModel();
			if (!_cpuCoresWasPressed) ToggleCpuCores();
			if (!_osVersionWasPressed) ToggleOsVersion();
			if (!_totalMemoryWasPressed) ToggleTotalMemory();
			if (!_deviceVendorWasPressed) ToggleDeviceVendor();
	}

    private void ClearAndDisposeTokens()
    {
        foreach (var token in _activeTokens)
        {
            if (token != null && !token.IsCancellationRequested)
            {
                token.Cancel();
                token.Dispose();
            }
        }

        _activeTokens.Clear();

        ResetState();
    }

    private void ResetState()
    {
        // Nullify individual token references
        _tokenDeviceModel = null;
        _tokenCpuCores = null;
        _tokenOsVersion = null;
        _tokenDeviceMemory = null;
        _tokenDeviceVendor = null;

        // Set the flags to false since the tokens were cleared/disposed
        _deviceModelWasPressed = false;
        _cpuCoresWasPressed = false;
        _osVersionWasPressed = false;
        _totalMemoryWasPressed = false;
        _deviceVendorWasPressed = false;
        
        _clearTextWasPressed = false;

        // Reset button texts to initial values
        deviceModelButtonText.text = "Get device model";
        cpuCoresButtonText.text = "Get CPU cores";
        osVersionButtonText.text = "Get OS version";
        totalMemoryButtonText.text = "Get total memory";
        deviceVendorButtonText.text = "Get device vendor";
		
        clearButtonText.text = "Clear all text";
        
        // Reset device information texts to empty
        unityDeviceModelText.text = string.Empty;
        unityCpuCoresText.text = string.Empty;
        unityOsVersionText.text = string.Empty;
        unityTotalMemoryText.text = string.Empty;
        unityDeviceVendorText.text = string.Empty;

        nativeDeviceModelText.text = string.Empty;
        nativeCpuCoresText.text = string.Empty;
        nativeOsVersionText.text = string.Empty;
        nativeTotalMemoryText.text = string.Empty;
        nativeDeviceVendorText.text = string.Empty;

        countdownText.text = string.Empty;
    }

	async void ToggleDeviceModel()
	{
		if (_tokenDeviceModel != null)
		{
			_tokenDeviceModel.Cancel();
			_tokenDeviceModel.Dispose();
			_tokenDeviceModel = null;
		}

		if (_deviceModelWasPressed)
		{
			_deviceModelWasPressed = false;
			deviceModelButtonText.text = "Get device model";
			unityDeviceModelText.text = string.Empty;
			nativeDeviceModelText.text = string.Empty;
			return;
		}
    
		try
		{
			_deviceModelWasPressed = true;
			deviceModelButtonText.text = "Clear device model";
			
			_tokenDeviceModel = new CancellationTokenSource();
			_activeTokens.Add(_tokenDeviceModel);
			var token = _tokenDeviceModel.Token;
        
			var unityDeviceModel = await _unityDeviceInfo.GetDeviceModel(token);
			unityDeviceModelText.text = string.Format(CultureInfo.InvariantCulture, "Device model: {0}", unityDeviceModel);
			
			nativeDeviceModelText.text =
				await GetNativeInfo(() => _nativeDeviceInfo.GetDeviceModel(token), "Device model");
		}
		catch (Exception ex)
		{
			Debug.LogError($"Error fetching device model: {ex.Message}");
		}
	}

    async void ToggleCpuCores()
	{
		if (_tokenCpuCores != null)
		{
			_tokenCpuCores.Cancel();
			_tokenCpuCores.Dispose();
			_tokenCpuCores = null;
		}

		if (_cpuCoresWasPressed)
		{
			_cpuCoresWasPressed = false;
			cpuCoresButtonText.text = "Get CPU cores";
			unityCpuCoresText.text = string.Empty;
			nativeCpuCoresText.text = string.Empty;
			return;
		}
    
		try
		{
			_cpuCoresWasPressed = true;
			cpuCoresButtonText.text = "Clear CPU cores";
			
			_tokenCpuCores = new CancellationTokenSource();
			_activeTokens.Add(_tokenCpuCores);
			var token = _tokenCpuCores.Token;
        
			var unityCpuCores = await _unityDeviceInfo.GetCpuCores(token);
			unityCpuCoresText.text = string.Format(CultureInfo.InvariantCulture, "CPU cores: {0}", unityCpuCores);
			
			nativeCpuCoresText.text =
				await GetNativeInfo(() => _nativeDeviceInfo.GetCpuCores(token), "CPU cores");
		}
		catch (Exception ex)
		{
			Debug.LogError($"Error fetching CPU cores: {ex.Message}");
		}
	}

	async void ToggleOsVersion()
	{
		if (_tokenOsVersion != null)
		{
			_tokenOsVersion.Cancel();
			_tokenOsVersion.Dispose();
			_tokenOsVersion = null;
		}

		if (_osVersionWasPressed)
		{
			_osVersionWasPressed = false;
			osVersionButtonText.text = "Get OS version";
			unityOsVersionText.text = string.Empty;
			nativeOsVersionText.text = string.Empty;
			return;
		}
    
		try
		{
			_osVersionWasPressed = true;
			osVersionButtonText.text = "Clear OS version";
			
			_tokenOsVersion = new CancellationTokenSource();
			_activeTokens.Add(_tokenOsVersion);
			var token = _tokenOsVersion.Token;
        
			var unityOsVersion = await _unityDeviceInfo.GetOsVersion(token);
			unityOsVersionText.text = string.Format(CultureInfo.InvariantCulture, "OS version: {0}", unityOsVersion);
			
			nativeOsVersionText.text =
				await GetNativeInfo(() => _nativeDeviceInfo.GetOsVersion(token), "OS version");
		}
		catch (Exception ex)
		{
			Debug.LogError($"Error fetching OS version: {ex.Message}");
		}
	}

	async void ToggleTotalMemory()
	{
		if (_tokenDeviceMemory != null)
		{
			_tokenDeviceMemory.Cancel();
			_tokenDeviceMemory.Dispose();
			_tokenDeviceMemory = null;
		}

		if (_totalMemoryWasPressed)
		{
			_totalMemoryWasPressed = false;
			totalMemoryButtonText.text = "Get total memory";
			unityTotalMemoryText.text = string.Empty;
			nativeTotalMemoryText.text = string.Empty;
			return;
		}
    
		try
		{
			_totalMemoryWasPressed = true;
			totalMemoryButtonText.text = "Clear total memory";
			
			_tokenDeviceMemory = new CancellationTokenSource();
			_activeTokens.Add(_tokenDeviceMemory);
			var token = _tokenDeviceMemory.Token;
        
			var unityDeviceMemory = await _unityDeviceInfo.GetDeviceMemory(token);
			unityTotalMemoryText.text = string.Format(CultureInfo.InvariantCulture, "Total memory: {0}", unityDeviceMemory);
			
			nativeTotalMemoryText.text =
				await GetNativeInfo(() => _nativeDeviceInfo.GetDeviceMemory(token), "Total memory");
		}
		catch (Exception ex)
		{
			Debug.LogError($"Error fetching total memory: {ex.Message}");
		}
	}

	async void ToggleDeviceVendor()
	{
		if (_tokenDeviceVendor != null)
		{
			_tokenDeviceVendor.Cancel();
			_tokenDeviceVendor.Dispose();
			_tokenDeviceVendor = null;
		}

		if (_deviceVendorWasPressed)
		{
			_deviceVendorWasPressed = false;
			deviceVendorButtonText.text = "Get device vendor";
			unityDeviceVendorText.text = string.Empty;
			nativeDeviceVendorText.text = string.Empty;
			return;
		}
    
		try
		{
			_deviceVendorWasPressed = true;
			deviceVendorButtonText.text = "Clear device vendor";
			
			_tokenDeviceVendor = new CancellationTokenSource();
			_activeTokens.Add(_tokenDeviceVendor);
			var token = _tokenDeviceVendor.Token;
        
			var unityDeviceVendor = await _unityDeviceInfo.GetDeviceVendor(token);
			unityDeviceVendorText.text = string.Format(CultureInfo.InvariantCulture, "Device vendor: {0}", unityDeviceVendor);
			
			nativeDeviceVendorText.text =
				await GetNativeInfo(() => _nativeDeviceInfo.GetDeviceVendor(token), "Device vendor");
		}
		catch (Exception ex)
		{
			Debug.LogError($"Error fetching device vendor: {ex.Message}");
		}
	}

	async Task<string> GetNativeInfo(Func<Task<string>> getInfoFunc, string infoName)
	{
		if (_nativeDeviceInfo == null)
		{
			return "Native API not found";
		}

		var info = await getInfoFunc();
		return string.IsNullOrEmpty(info) ? "Native API not found" : $"{infoName}: {info}";
	}

	async Task<string> GetNativeInfo(Func<Task<int>> getInfoFunc, string infoName)
	{
		if (_nativeDeviceInfo == null)
		{
			return "Native API not found";
		}

		var info = await getInfoFunc();
		return $"{infoName}: {info}";
	}

	async Task<string> GetNativeInfo(Func<Task<long>> getInfoFunc, string infoName)
	{
		if (_nativeDeviceInfo == null)
		{
			return "Native API not found";
		}

		var info = await getInfoFunc();
		return $"{infoName}: {info}";
	}

	IEnumerator CountdownTimer()
	{
		int counter = 3;
    
		while (counter > 0)
		{
			countdownText.text = counter.ToString();
			clearButtonText.text = "Cancel action";
			yield return new WaitForSeconds(1);
			counter--;
		}
		countdownText.text = string.Empty;
	}

	async void ClearAfterDelay()
	{
		if (_tokenClearText != null)
		{
			_tokenClearText.Cancel();
			_tokenClearText.Dispose();
			_tokenClearText = null;
		}
		
		if (_clearTextWasPressed)
		{
			StopAllCoroutines();
			_clearTextWasPressed = false;
			clearButtonText.text = "Clear all text";
			countdownText.text = string.Empty;
			return;
		}

		_tokenClearText = new CancellationTokenSource();
		_clearTextWasPressed = true;
		StartCoroutine(CountdownTimer());
		
		try
		{
			await Task.Delay(3000, _tokenClearText.Token);
			
			ClearAndDisposeTokens();
			ClearAllText();
			_clearTextWasPressed = false;
		}
		catch (TaskCanceledException)
		{
			_tokenClearText = new CancellationTokenSource();
			StopAllCoroutines();
			countdownText.text = string.Empty;
		}
	}
}