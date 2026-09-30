/// Builds a greeting for [name], or null when [name] is blank.
String? buildGreeting(String? name) {
  final trimmed = name?.trim() ?? '';
  if (trimmed.isEmpty) return null;
  return 'Hello, $trimmed!';
}
