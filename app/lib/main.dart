import 'package:flutter/material.dart';

import 'greeting.dart';

void main() => runApp(const HelloApp());

class HelloApp extends StatelessWidget {
  const HelloApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Hello App',
      theme: ThemeData(colorSchemeSeed: Colors.teal, useMaterial3: true),
      home: const HomePage(),
    );
  }
}

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  final _controller = TextEditingController();
  String _message = 'Enter your name';

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _greet() {
    setState(() {
      _message = buildGreeting(_controller.text) ?? 'Name must not be blank.';
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Hello App')),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          children: [
            TextField(
              controller: _controller,
              decoration: const InputDecoration(labelText: 'Name'),
            ),
            const SizedBox(height: 16),
            FilledButton(onPressed: _greet, child: const Text('Greet')),
            const SizedBox(height: 24),
            Text(_message, key: const Key('message')),
          ],
        ),
      ),
    );
  }
}
