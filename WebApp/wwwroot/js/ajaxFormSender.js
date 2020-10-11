async function sendAJAX(event, successFunction)
{
	event.preventDefault();
	let actionElement = event.target //document.querySelector(formSelector);
	let tagName = actionElement.tagName.toLowerCase();
	let requestUrl = actionElement.getAttribute("action");
	let submitMethod = actionElement.getAttribute("method");
	let enctype = actionElement.getAttribute("enctype");
	
	let formData;
	
	if( tagName == 'form')
	{
		if (!requestUrl)
			requestUrl = window.location.pathname;

		if (!submitMethod)
			submitMethod = "POST";
		
		if (enctype == "multipart/form-data")
			formData = new FormData(actionElement);
		else
			formData = new URLSearchParams(new FormData(actionElement))
	}
	else if(tagName == "input")
	{
		if (!requestUrl)
			requestUrl = window.location.pathname;

		if (!submitMethod)
			submitMethod = "POST";
		
		formData = new FormData();
		var name = actionElement.getAttribute("name");
		var value = actionElement.value;
		formData.append(name, value);
		
		if (enctype == "multipart/form-data")
			formData = new FormData();
		else
			formData = new URLSearchParams(formData)
	}
	else if(tagName == "a")
	{
		if (!requestUrl)
			requestUrl = actionElement.getAttribute("href");

		if (!submitMethod)
			submitMethod = "GET";
	}	
	

	try
	{
		const response = await fetch(requestUrl,
		{
			method: submitMethod,
			body: formData,
		});

		if (response.redirected)
			window.location.href = response.url;
		else
		{
			let data;
			const contentType = response.headers.get('content-type');
			if (!contentType || !contentType.includes('application/json')) 
			{
				data = await response.text()
			}
			else
			{				
				data = await response.json();
			}
			
			if (!response.ok)
			{			
				if (data.errors)
					for (error of data.errors)
					{
						alert(error);
					}
			}
			else
			{
				if (successFunction)
					successFunction(data);
				else
					console.log('Success:', data);
				
				let reset = actionElement.getAttribute("reset");
				
				if(reset != "false")
				{
					if(tagName == "form")
						actionElement.reset();
					else if(tagName == "input")
						actionElement.value = "";
				}
				
				
			}
		}		
	}
	catch (error)
	{
		alert("An error occured please try again!");
		console.error(error);
	}        
}