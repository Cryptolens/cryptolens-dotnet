## Changes since previous release

### Proxy configuration
Some customers may be running through a proxy, in which case it will not allow them to access `SKM Web API`. To fix this, create a new file, called `config.json`, with the following content:

```
{
    "proxy": "default"
}
```