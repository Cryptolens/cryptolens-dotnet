---
title: Pitfalls in v401
---

# Pitfalls
* Keep in mind the difference between **GetKeys** and **GetKey** methods. They both require different scope and cannot be used interchangeably.
* Don't increment Data Objects right away. These changes will be lost. Instead, use the IncrementIntValue value method. The same goes for changing other fields, such as the StringValue and IntValue.
* The `license.Refresh()` returns either `true` or `false`. If `false`, there's an error. A good practise is to do the following
```
if(!license.Refresh(token))
    // could not refresh. check the permissions. It's GetKey, not GetKeys!
```
